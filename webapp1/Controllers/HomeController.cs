using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapp1.Data;
using webapp1.Models;
using webapp1.Services;

namespace webapp1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ICountryService _countryService;
        private readonly IPdfExportService _pdfExportService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, ICountryService countryService, IPdfExportService pdfExportService)
        {
            _logger = logger;
            _context = context;
            _countryService = countryService;
            _pdfExportService = pdfExportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
            
            return View(products);
        }

        public async Task<IActionResult> Countries()
        {
            try
            {
                var countries = await _countryService.GetCountriesAsync();
                return View(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading countries page");
                return View(new List<Country>());
            }
        }

        public async Task<IActionResult> ExportProductsToPdf()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                var pdfBytes = _pdfExportService.GenerateProductsPdf(products);
                
                return File(pdfBytes, "application/pdf", "products-export.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating products PDF");
                return RedirectToAction("Products");
            }
        }

        public async Task<IActionResult> ExportCountriesToPdf()
        {
            try
            {
                var countries = await _countryService.GetCountriesAsync();
                var pdfBytes = _pdfExportService.GenerateCountriesPdf(countries);
                
                return File(pdfBytes, "application/pdf", "countries-export.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating countries PDF");
                return RedirectToAction("Countries");
            }
        }

        public IActionResult TestPdf()
        {
            try
            {
                // Test with sample data
                var testProducts = new List<Product>
                {
                    new Product { Id = 1, Name = "Test Product", Category = "Test", Price = 99.99m, CreatedDate = DateTime.Now, IsActive = true }
                };

                var pdfBytes = _pdfExportService.GenerateProductsPdf(testProducts);
                
                return File(pdfBytes, "application/pdf", "test-export.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test PDF");
                return Content($"PDF Test Failed: {ex.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
