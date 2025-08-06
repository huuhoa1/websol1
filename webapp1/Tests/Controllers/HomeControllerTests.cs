using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using webapp1.Controllers;
using webapp1.Data;
using webapp1.Models;
using webapp1.Services;

namespace webapp1.Tests.Controllers
{
    public class HomeControllerTests : IDisposable
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<ICountryService> _mockCountryService;
        private readonly Mock<IPdfExportService> _mockPdfExportService;
        private readonly ApplicationDbContext _context;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockCountryService = new Mock<ICountryService>();
            _mockPdfExportService = new Mock<IPdfExportService>();

            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed test data
            SeedTestData();

            _controller = new HomeController(_mockLogger.Object, _context, _mockCountryService.Object, _mockPdfExportService.Object);
        }

        private void SeedTestData()
        {
            var products = new List<Product>
            {
                new()
                {
                    Id = 1,
                    Name = "Test Product 1",
                    Category = "Electronics",
                    Price = 99.99m,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    IsActive = true
                },
                new()
                {
                    Id = 2,
                    Name = "Test Product 2",
                    Category = "Books",
                    Price = 19.99m,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new()
                {
                    Id = 3,
                    Name = "Inactive Product",
                    Category = "Test",
                    Price = 49.99m,
                    CreatedDate = DateTime.Now.AddDays(-2),
                    IsActive = false
                }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Products_ReturnsViewWithActiveProducts()
        {
            // Act
            var result = await _controller.Products();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            Assert.Equal(2, model.Count);
            Assert.All(model, p => Assert.True(p.IsActive));
            Assert.Equal("Test Product 1", model[0].Name); // Should be ordered by name
            Assert.Equal("Test Product 2", model[1].Name);
        }

        [Fact]
        public async Task Countries_WithSuccessfulService_ReturnsViewWithCountries()
        {
            // Arrange
            var expectedCountries = new List<Country>
            {
                new() { Code = "US", Name = "United States", Continent = new Continent { Name = "North America" } },
                new() { Code = "CA", Name = "Canada", Continent = new Continent { Name = "North America" } }
            };

            _mockCountryService
                .Setup(s => s.GetCountriesAsync())
                .ReturnsAsync(expectedCountries);

            // Act
            var result = await _controller.Countries();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Country>>(viewResult.Model);
            
            Assert.Equal(2, model.Count);
            Assert.Equal("US", model[0].Code);
            Assert.Equal("CA", model[1].Code);
        }

        [Fact]
        public async Task Countries_WithServiceException_ReturnsViewWithEmptyList()
        {
            // Arrange
            _mockCountryService
                .Setup(s => s.GetCountriesAsync())
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Countries();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Country>>(viewResult.Model);
            
            Assert.Empty(model);

            // Verify error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error loading countries page")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExportProductsToPdf_WithSuccessfulGeneration_ReturnsFileResult()
        {
            // Arrange
            var expectedPdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF header
            _mockPdfExportService
                .Setup(s => s.GenerateProductsPdf(It.IsAny<IEnumerable<Product>>()))
                .Returns(expectedPdfBytes);

            // Act
            var result = await _controller.ExportProductsToPdf();

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal("products-export.pdf", fileResult.FileDownloadName);
            Assert.Equal(expectedPdfBytes, fileResult.FileContents);

            // Verify PDF service was called with active products
            _mockPdfExportService.Verify(
                s => s.GenerateProductsPdf(It.Is<IEnumerable<Product>>(products => 
                    products.Count() == 2 && products.All(p => p.IsActive))),
                Times.Once);
        }

        [Fact]
        public async Task ExportProductsToPdf_WithServiceException_RedirectsToProducts()
        {
            // Arrange
            _mockPdfExportService
                .Setup(s => s.GenerateProductsPdf(It.IsAny<IEnumerable<Product>>()))
                .Throws(new Exception("PDF generation error"));

            // Act
            var result = await _controller.ExportProductsToPdf();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Products", redirectResult.ActionName);

            // Verify error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error generating products PDF")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExportCountriesToPdf_WithSuccessfulGeneration_ReturnsFileResult()
        {
            // Arrange
            var expectedCountries = new List<Country>
            {
                new() { Code = "US", Name = "United States", Continent = new Continent { Name = "North America" } }
            };
            var expectedPdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF header

            _mockCountryService
                .Setup(s => s.GetCountriesAsync())
                .ReturnsAsync(expectedCountries);

            _mockPdfExportService
                .Setup(s => s.GenerateCountriesPdf(It.IsAny<IEnumerable<Country>>()))
                .Returns(expectedPdfBytes);

            // Act
            var result = await _controller.ExportCountriesToPdf();

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal("countries-export.pdf", fileResult.FileDownloadName);
            Assert.Equal(expectedPdfBytes, fileResult.FileContents);

            // Verify services were called
            _mockCountryService.Verify(s => s.GetCountriesAsync(), Times.Once);
            _mockPdfExportService.Verify(s => s.GenerateCountriesPdf(expectedCountries), Times.Once);
        }

        [Fact]
        public async Task ExportCountriesToPdf_WithServiceException_RedirectsToCountries()
        {
            // Arrange
            _mockCountryService
                .Setup(s => s.GetCountriesAsync())
                .ThrowsAsync(new Exception("Country service error"));

            // Act
            var result = await _controller.ExportCountriesToPdf();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Countries", redirectResult.ActionName);

            // Verify error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error generating countries PDF")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void TestPdf_WithSuccessfulGeneration_ReturnsFileResult()
        {
            // Arrange
            var expectedPdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF header
            _mockPdfExportService
                .Setup(s => s.GenerateProductsPdf(It.IsAny<IEnumerable<Product>>()))
                .Returns(expectedPdfBytes);

            // Act
            var result = _controller.TestPdf();

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal("test-export.pdf", fileResult.FileDownloadName);
            Assert.Equal(expectedPdfBytes, fileResult.FileContents);

            // Verify PDF service was called with test data
            _mockPdfExportService.Verify(
                s => s.GenerateProductsPdf(It.Is<IEnumerable<Product>>(products => 
                    products.Count() == 1 && products.First().Name == "Test Product")),
                Times.Once);
        }

        [Fact]
        public void TestPdf_WithServiceException_ReturnsContentResult()
        {
            // Arrange
            var expectedException = new Exception("PDF test error");
            _mockPdfExportService
                .Setup(s => s.GenerateProductsPdf(It.IsAny<IEnumerable<Product>>()))
                .Throws(expectedException);

            // Act
            var result = _controller.TestPdf();

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Contains("PDF Test Failed", contentResult.Content);
            Assert.Contains("PDF test error", contentResult.Content);

            // Verify error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error generating test PDF")),
                    expectedException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test-trace-id";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
            Assert.Equal("test-trace-id", model.RequestId);
        }

        [Fact]
        public async Task Products_FiltersOutInactiveProducts()
        {
            // Act
            var result = await _controller.Products();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Should not include the inactive product with ID 3
            Assert.DoesNotContain(model, p => p.Name == "Inactive Product");
            Assert.All(model, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task Products_OrdersByName()
        {
            // Act
            var result = await _controller.Products();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Should be ordered alphabetically
            var names = model.Select(p => p.Name).ToList();
            var sortedNames = names.OrderBy(n => n).ToList();
            Assert.Equal(sortedNames, names);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}