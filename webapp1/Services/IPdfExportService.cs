using webapp1.Models;

namespace webapp1.Services
{
    public interface IPdfExportService
    {
        byte[] GenerateProductsPdf(IEnumerable<Product> products);
        byte[] GenerateCountriesPdf(IEnumerable<Country> countries);
    }
}