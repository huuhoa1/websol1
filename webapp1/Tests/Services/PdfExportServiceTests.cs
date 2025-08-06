using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using webapp1.Services;
using webapp1.Models;

namespace webapp1.Tests.Services
{
    public class PdfExportServiceTests
    {
        private readonly Mock<ILogger<PdfExportService>> _mockLogger;
        private readonly PdfExportService _pdfExportService;

        public PdfExportServiceTests()
        {
            _mockLogger = new Mock<ILogger<PdfExportService>>();
            _pdfExportService = new PdfExportService(_mockLogger.Object);
        }

        [Fact]
        public void GenerateProductsPdf_WithEmptyList_ReturnsPdfBytes()
        {
            // Arrange
            var products = new List<Product>();

            // Act
            var result = _pdfExportService.GenerateProductsPdf(products);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            
            // Verify PDF header (PDF files start with %PDF-)
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result, 0, 4);
            Assert.Equal("%PDF", pdfHeader);
        }

        [Fact]
        public void GenerateProductsPdf_WithSingleProduct_ReturnsPdfBytes()
        {
            // Arrange
            var products = new List<Product>
            {
                new()
                {
                    Id = 1,
                    Name = "Test Product",
                    Category = "Electronics",
                    Price = 99.99m,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            };

            // Act
            var result = _pdfExportService.GenerateProductsPdf(products);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            
            // Verify PDF header
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result, 0, 4);
            Assert.Equal("%PDF", pdfHeader);
            
            // Verify logging was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting PDF generation for products")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void GenerateProductsPdf_WithMultipleProducts_ReturnsPdfBytes()
        {
            // Arrange
            var products = new List<Product>
            {
                new()
                {
                    Id = 1,
                    Name = "Product 1",
                    Category = "Electronics",
                    Price = 99.99m,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    IsActive = true
                },
                new()
                {
                    Id = 2,
                    Name = "Product 2",
                    Category = "Books",
                    Price = 19.99m,
                    CreatedDate = DateTime.Now,
                    IsActive = false
                }
            };

            // Act
            var result = _pdfExportService.GenerateProductsPdf(products);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            
            // Verify PDF header
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result, 0, 4);
            Assert.Equal("%PDF", pdfHeader);
        }

        [Fact]
        public void GenerateCountriesPdf_WithEmptyList_ReturnsPdfBytes()
        {
            // Arrange
            var countries = new List<Country>();

            // Act
            var result = _pdfExportService.GenerateCountriesPdf(countries);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            
            // Verify PDF header
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result, 0, 4);
            Assert.Equal("%PDF", pdfHeader);
        }

        [Fact]
        public void GenerateCountriesPdf_WithSingleCountry_ReturnsPdfBytes()
        {
            // Arrange
            var countries = new List<Country>
            {
                new()
                {
                    Code = "US",
                    Name = "United States",
                    Capital = "Washington, D.C.",
                    Currency = "USD",
                    Phone = "+1",
                    Continent = new Continent { Code = "NA", Name = "North America" }
                }
            };

            // Act
            var result = _pdfExportService.GenerateCountriesPdf(countries);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            
            // Verify PDF header
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result, 0, 4);
            Assert.Equal("%PDF", pdfHeader);
            
            // Verify logging was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting PDF generation for countries")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void GenerateCountriesPdf_WithMultipleCountries_ReturnsPdfBytes()
        {
            // Arrange
            var countries = new List<Country>
            {
                new()
                {
                    Code = "US",
                    Name = "United States",
                    Capital = "Washington, D.C.",
                    Currency = "USD",
                    Phone = "+1",
                    Continent = new Continent { Code = "NA", Name = "North America" }
                },
                new()
                {
                    Code = "CA",
                    Name = "Canada",
                    Capital = "Ottawa",
                    Currency = "CAD",
                    Phone = "+1",
                    Continent = new Continent { Code = "NA", Name = "North America" }
                }
            };

            // Act
            var result = _pdfExportService.GenerateCountriesPdf(countries);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            
            // Verify PDF header
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result, 0, 4);
            Assert.Equal("%PDF", pdfHeader);
        }

        [Fact]
        public void GenerateCountriesPdf_WithNullOrEmptyFields_HandlesProperly()
        {
            // Arrange
            var countries = new List<Country>
            {
                new()
                {
                    Code = "XX",
                    Name = "Test Country",
                    Capital = null!,
                    Currency = "",
                    Phone = null!,
                    Continent = new Continent { Code = "XX", Name = "Test Continent" }
                }
            };

            // Act
            var result = _pdfExportService.GenerateCountriesPdf(countries);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            
            // Verify PDF header
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result, 0, 4);
            Assert.Equal("%PDF", pdfHeader);
        }

        [Fact]
        public void GenerateProductsPdf_LogsInformationMessages()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 1, Name = "Test", Category = "Test", Price = 10m, CreatedDate = DateTime.Now, IsActive = true }
            };

            // Act
            _pdfExportService.GenerateProductsPdf(products);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing 1 products")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("PDF generated successfully")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void GenerateCountriesPdf_LogsInformationMessages()
        {
            // Arrange
            var countries = new List<Country>
            {
                new()
                {
                    Code = "US",
                    Name = "United States",
                    Continent = new Continent { Name = "North America" }
                }
            };

            // Act
            _pdfExportService.GenerateCountriesPdf(countries);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing 1 countries")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("PDF generated successfully")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}