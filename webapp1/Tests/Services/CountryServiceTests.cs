using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using GraphQL.Client.Abstractions;
using GraphQL;
using webapp1.Services;
using webapp1.Models;

namespace webapp1.Tests.Services
{
    public class CountryServiceTests
    {
        private readonly Mock<IGraphQLClient> _mockGraphQLClient;
        private readonly Mock<ILogger<CountryService>> _mockLogger;
        private readonly CountryService _countryService;

        public CountryServiceTests()
        {
            _mockGraphQLClient = new Mock<IGraphQLClient>();
            _mockLogger = new Mock<ILogger<CountryService>>();
            _countryService = new CountryService(_mockGraphQLClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCountriesAsync_WithSuccessfulResponse_ReturnsCountries()
        {
            // Arrange
            var expectedCountries = new List<Country>
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

            var mockResponse = new GraphQLResponse<CountriesResponse>
            {
                Data = new CountriesResponse { Countries = expectedCountries }
            };

            _mockGraphQLClient
                .Setup(x => x.SendQueryAsync<CountriesResponse>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _countryService.GetCountriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("US", result[0].Code);
            Assert.Equal("United States", result[0].Name);
            Assert.Equal("CA", result[1].Code);
            Assert.Equal("Canada", result[1].Name);
        }

        [Fact]
        public async Task GetCountriesAsync_WithEmptyResponse_ReturnsEmptyList()
        {
            // Arrange
            var mockResponse = new GraphQLResponse<CountriesResponse>
            {
                Data = new CountriesResponse { Countries = new List<Country>() }
            };

            _mockGraphQLClient
                .Setup(x => x.SendQueryAsync<CountriesResponse>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _countryService.GetCountriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCountriesAsync_WithNullData_ReturnsEmptyList()
        {
            // Arrange
            var mockResponse = new GraphQLResponse<CountriesResponse>
            {
                Data = null
            };

            _mockGraphQLClient
                .Setup(x => x.SendQueryAsync<CountriesResponse>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _countryService.GetCountriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCountriesAsync_WithGraphQLErrors_ReturnsEmptyListAndLogsError()
        {
            // Arrange
            var mockResponse = new GraphQLResponse<CountriesResponse>
            {
                Errors = new[]
                {
                    new GraphQLError { Message = "Test error 1" },
                    new GraphQLError { Message = "Test error 2" }
                }
            };

            _mockGraphQLClient
                .Setup(x => x.SendQueryAsync<CountriesResponse>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _countryService.GetCountriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            // Verify error logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("GraphQL errors") && 
                                                v.ToString()!.Contains("Test error 1") && 
                                                v.ToString()!.Contains("Test error 2")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCountriesAsync_WithException_ReturnsEmptyListAndLogsError()
        {
            // Arrange
            var expectedException = new Exception("Network error");

            _mockGraphQLClient
                .Setup(x => x.SendQueryAsync<CountriesResponse>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act
            var result = await _countryService.GetCountriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            // Verify exception logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error fetching countries from GraphQL API")),
                    expectedException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCountriesAsync_SendsCorrectGraphQLRequest()
        {
            // Arrange
            var mockResponse = new GraphQLResponse<CountriesResponse>
            {
                Data = new CountriesResponse { Countries = new List<Country>() }
            };

            GraphQLRequest? capturedRequest = null;
            _mockGraphQLClient
                .Setup(x => x.SendQueryAsync<CountriesResponse>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .Callback<GraphQLRequest, CancellationToken>((request, token) => capturedRequest = request)
                .ReturnsAsync(mockResponse);

            // Act
            await _countryService.GetCountriesAsync();

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.NotNull(capturedRequest.Query);
            Assert.Contains("countries", capturedRequest.Query);
        }

        [Fact]
        public async Task GetCountriesAsync_WithCompleteCountryData_MapsAllProperties()
        {
            // Arrange
            var expectedCountries = new List<Country>
            {
                new()
                {
                    Code = "FR",
                    Name = "France",
                    Capital = "Paris",
                    Currency = "EUR",
                    Phone = "+33",
                    Continent = new Continent { Code = "EU", Name = "Europe" }
                }
            };

            var mockResponse = new GraphQLResponse<CountriesResponse>
            {
                Data = new CountriesResponse { Countries = expectedCountries }
            };

            _mockGraphQLClient
                .Setup(x => x.SendQueryAsync<CountriesResponse>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _countryService.GetCountriesAsync();

            // Assert
            Assert.Single(result);
            var country = result[0];
            Assert.Equal("FR", country.Code);
            Assert.Equal("France", country.Name);
            Assert.Equal("Paris", country.Capital);
            Assert.Equal("EUR", country.Currency);
            Assert.Equal("+33", country.Phone);
            Assert.NotNull(country.Continent);
            Assert.Equal("EU", country.Continent.Code);
            Assert.Equal("Europe", country.Continent.Name);
        }

        [Fact]
        public async Task GetCountriesAsync_WithPartialCountryData_HandlesNullFields()
        {
            // Arrange
            var expectedCountries = new List<Country>
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

            var mockResponse = new GraphQLResponse<CountriesResponse>
            {
                Data = new CountriesResponse { Countries = expectedCountries }
            };

            _mockGraphQLClient
                .Setup(x => x.SendQueryAsync<CountriesResponse>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _countryService.GetCountriesAsync();

            // Assert
            Assert.Single(result);
            var country = result[0];
            Assert.Equal("XX", country.Code);
            Assert.Equal("Test Country", country.Name);
            Assert.Null(country.Capital);
            Assert.Equal("", country.Currency);
            Assert.Null(country.Phone);
            Assert.NotNull(country.Continent);
        }
    }
}