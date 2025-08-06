using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using webapp1.Models;

namespace webapp1.Services
{
    public class PdfExportService : IPdfExportService
    {
        private readonly ILogger<PdfExportService> _logger;

        public PdfExportService(ILogger<PdfExportService> logger)
        {
            _logger = logger;
        }
        public byte[] GenerateProductsPdf(IEnumerable<Product> products)
        {
            try
            {
                _logger.LogInformation("Starting PDF generation for products");
                var productsList = products.ToList();
                _logger.LogInformation($"Processing {productsList.Count} products");

                var stream = new MemoryStream();
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Add title
                document.Add(new Paragraph("Products Report")
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER));

                // Create table
                var table = new Table(6);
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Add headers
                table.AddHeaderCell("ID");
                table.AddHeaderCell("Name");
                table.AddHeaderCell("Category");
                table.AddHeaderCell("Price");
                table.AddHeaderCell("Created Date");
                table.AddHeaderCell("Status");

                // Add data rows
                foreach (var product in productsList)
                {
                    table.AddCell(product.Id.ToString());
                    table.AddCell(product.Name);
                    table.AddCell(product.Category);
                    table.AddCell(product.Price.ToString("C"));
                    table.AddCell(product.CreatedDate.ToString("MMM dd, yyyy"));
                    table.AddCell("Active");
                }

                document.Add(table);
                document.Close();
                pdf.Close();
                writer.Close();

                var result = stream.ToArray();
                _logger.LogInformation($"PDF generated successfully, size: {result.Length} bytes");
                stream.Close();
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating products PDF");
                throw;
            }
        }

        public byte[] GenerateCountriesPdf(IEnumerable<Country> countries)
        {
            try
            {
                _logger.LogInformation("Starting PDF generation for countries");
                var countriesList = countries.ToList();
                _logger.LogInformation($"Processing {countriesList.Count} countries");

                var stream = new MemoryStream();
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Add title
                document.Add(new Paragraph("Countries Report")
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER));

                // Create table
                var table = new Table(6);
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Add headers
                table.AddHeaderCell("Code");
                table.AddHeaderCell("Name");
                table.AddHeaderCell("Capital");
                table.AddHeaderCell("Currency");
                table.AddHeaderCell("Phone");
                table.AddHeaderCell("Continent");

                // Add data rows
                foreach (var country in countriesList)
                {
                    table.AddCell(country.Code);
                    table.AddCell(country.Name);
                    table.AddCell(string.IsNullOrEmpty(country.Capital) ? "-" : country.Capital);
                    table.AddCell(string.IsNullOrEmpty(country.Currency) ? "-" : country.Currency);
                    table.AddCell(string.IsNullOrEmpty(country.Phone) ? "-" : country.Phone);
                    table.AddCell(country.Continent.Name);
                }

                document.Add(table);
                document.Close();
                pdf.Close();
                writer.Close();

                var result = stream.ToArray();
                _logger.LogInformation($"PDF generated successfully, size: {result.Length} bytes");
                stream.Close();
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating countries PDF");
                throw;
            }
        }
    }
}