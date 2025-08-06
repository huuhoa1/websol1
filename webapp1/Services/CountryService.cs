using GraphQL.Client.Abstractions;
using GraphQL;
using webapp1.Models;
using webapp1.GraphQL;

namespace webapp1.Services
{
    public interface ICountryService
    {
        Task<List<Country>> GetCountriesAsync();
    }

    public class CountryService : ICountryService
    {
        private readonly IGraphQLClient _graphQLClient;
        private readonly ILogger<CountryService> _logger;

        public CountryService(IGraphQLClient graphQLClient, ILogger<CountryService> logger)
        {
            _graphQLClient = graphQLClient;
            _logger = logger;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            try
            {
                var request = new GraphQLRequest
                {
                    Query = CountryQueries.GetAllCountries
                };

                var response = await _graphQLClient.SendQueryAsync<CountriesResponse>(request);

                if (response.Errors?.Any() == true)
                {
                    _logger.LogError("GraphQL errors: {Errors}", string.Join(", ", response.Errors.Select(e => e.Message)));
                    return new List<Country>();
                }

                return response.Data?.Countries ?? new List<Country>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching countries from GraphQL API");
                return new List<Country>();
            }
        }
    }
}