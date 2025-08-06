namespace webapp1.GraphQL
{
    public static class CountryQueries
    {
        public const string GetAllCountries = @"
            query GetAllCountries {
                countries {
                    code
                    name
                    capital
                    currency
                    phone
                    continent {
                        code
                        name
                    }
                }
            }";
    }
}