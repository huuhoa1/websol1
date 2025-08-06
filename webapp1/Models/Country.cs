namespace webapp1.Models
{
    public class Country
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Capital { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Continent Continent { get; set; } = new();
    }

    public class Continent
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CountriesResponse
    {
        public List<Country> Countries { get; set; } = new();
    }
}