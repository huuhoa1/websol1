using Xunit;
using webapp1.Models;

namespace webapp1.Tests.Models
{
    public class CountryTests
    {
        [Fact]
        public void Country_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var country = new Country();

            // Assert
            Assert.Equal(string.Empty, country.Code);
            Assert.Equal(string.Empty, country.Name);
            Assert.Equal(string.Empty, country.Capital);
            Assert.Equal(string.Empty, country.Currency);
            Assert.Equal(string.Empty, country.Phone);
            Assert.NotNull(country.Continent);
        }

        [Fact]
        public void Country_PropertyAssignment_SetsCorrectValues()
        {
            // Arrange
            var country = new Country();
            var continent = new Continent { Code = "NA", Name = "North America" };

            // Act
            country.Code = "US";
            country.Name = "United States";
            country.Capital = "Washington, D.C.";
            country.Currency = "USD";
            country.Phone = "+1";
            country.Continent = continent;

            // Assert
            Assert.Equal("US", country.Code);
            Assert.Equal("United States", country.Name);
            Assert.Equal("Washington, D.C.", country.Capital);
            Assert.Equal("USD", country.Currency);
            Assert.Equal("+1", country.Phone);
            Assert.Equal(continent, country.Continent);
            Assert.Equal("NA", country.Continent.Code);
            Assert.Equal("North America", country.Continent.Name);
        }

        [Theory]
        [InlineData("US", "United States")]
        [InlineData("CA", "Canada")]
        [InlineData("FR", "France")]
        public void Country_CodeAndName_AcceptValidValues(string code, string name)
        {
            // Arrange
            var country = new Country();

            // Act
            country.Code = code;
            country.Name = name;

            // Assert
            Assert.Equal(code, country.Code);
            Assert.Equal(name, country.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Washington, D.C.")]
        [InlineData("London")]
        [InlineData("Paris")]
        public void Country_Capital_AcceptsVariousValues(string capital)
        {
            // Arrange
            var country = new Country();

            // Act
            country.Capital = capital;

            // Assert
            Assert.Equal(capital, country.Capital);
        }

        [Theory]
        [InlineData("")]
        [InlineData("USD")]
        [InlineData("EUR")]
        [InlineData("GBP")]
        public void Country_Currency_AcceptsVariousValues(string currency)
        {
            // Arrange
            var country = new Country();

            // Act
            country.Currency = currency;

            // Assert
            Assert.Equal(currency, country.Currency);
        }

        [Theory]
        [InlineData("")]
        [InlineData("+1")]
        [InlineData("+44")]
        [InlineData("+33")]
        public void Country_Phone_AcceptsVariousValues(string phone)
        {
            // Arrange
            var country = new Country();

            // Act
            country.Phone = phone;

            // Assert
            Assert.Equal(phone, country.Phone);
        }
    }

    public class ContinentTests
    {
        [Fact]
        public void Continent_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var continent = new Continent();

            // Assert
            Assert.Equal(string.Empty, continent.Code);
            Assert.Equal(string.Empty, continent.Name);
        }

        [Fact]
        public void Continent_PropertyAssignment_SetsCorrectValues()
        {
            // Arrange
            var continent = new Continent();

            // Act
            continent.Code = "EU";
            continent.Name = "Europe";

            // Assert
            Assert.Equal("EU", continent.Code);
            Assert.Equal("Europe", continent.Name);
        }

        [Theory]
        [InlineData("NA", "North America")]
        [InlineData("EU", "Europe")]
        [InlineData("AS", "Asia")]
        [InlineData("AF", "Africa")]
        [InlineData("OC", "Oceania")]
        [InlineData("SA", "South America")]
        [InlineData("AN", "Antarctica")]
        public void Continent_CodeAndName_AcceptValidValues(string code, string name)
        {
            // Arrange
            var continent = new Continent();

            // Act
            continent.Code = code;
            continent.Name = name;

            // Assert
            Assert.Equal(code, continent.Code);
            Assert.Equal(name, continent.Name);
        }
    }

    public class CountriesResponseTests
    {
        [Fact]
        public void CountriesResponse_DefaultConstructor_InitializesEmptyList()
        {
            // Arrange & Act
            var response = new CountriesResponse();

            // Assert
            Assert.NotNull(response.Countries);
            Assert.Empty(response.Countries);
        }

        [Fact]
        public void CountriesResponse_Countries_CanAddAndRetrieveCountries()
        {
            // Arrange
            var response = new CountriesResponse();
            var country1 = new Country { Code = "US", Name = "United States" };
            var country2 = new Country { Code = "CA", Name = "Canada" };

            // Act
            response.Countries.Add(country1);
            response.Countries.Add(country2);

            // Assert
            Assert.Equal(2, response.Countries.Count);
            Assert.Contains(country1, response.Countries);
            Assert.Contains(country2, response.Countries);
        }

        [Fact]
        public void CountriesResponse_Countries_CanBeSetDirectly()
        {
            // Arrange
            var response = new CountriesResponse();
            var countries = new List<Country>
            {
                new() { Code = "US", Name = "United States" },
                new() { Code = "CA", Name = "Canada" },
                new() { Code = "MX", Name = "Mexico" }
            };

            // Act
            response.Countries = countries;

            // Assert
            Assert.Equal(3, response.Countries.Count);
            Assert.Equal(countries, response.Countries);
        }
    }
}