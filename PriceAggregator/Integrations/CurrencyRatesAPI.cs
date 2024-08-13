using Microsoft.AspNetCore.WebUtilities;

namespace PriceAggregator.Integrations
{
    public interface ICurrencyRatesAPI
    {
        Task<string?> GetCurrencyRate(string baseCurrency, string quoteCurrency, DateTime startDate, DateTime endDate);
        Task<List<Currency>?> GetCurrencyRates();
    }

    public class CurrencyRatesAPI : ICurrencyRatesAPI
    {

        public async Task<string?> GetCurrencyRate(string baseCurrency, string quoteCurrency, DateTime startDate, DateTime endDate)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://fxds-public-exchange-rates-api.oanda.com/cc-api/currencies");
            var parameters = new Dictionary<string, string?>
            {
                { "base", baseCurrency },
                { "quote", quoteCurrency },
                { "data_type", "general_currency_pair" },
                { "start_date", startDate.Date.ToString("yyyy-MM-dd") },
                { "end_date", endDate.Date.ToString("yyyy-MM-dd") }
            };
            var query = QueryHelpers.AddQueryString(string.Empty, parameters);
            var response = await client.GetFromJsonAsync<CurrenciesResponse>(query);
            return response?.response.FirstOrDefault()?.average_ask;
        }
        public async Task<List<Currency>?> GetCurrencyRates()
        {
            using var client = new HttpClient();
            var response = await client.GetFromJsonAsync<List<Root>>(new Uri("https://nbg.gov.ge/gw/api/ct/monetarypolicy/currencies/en/json/"));
            return response?.First().currencies;
        }

    }

    public class Currency
    {
        public string code { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public double rate { get; set; }
        public DateTime date { get; set; }
        public DateTime validFromDate { get; set; }
    }

    class Root
    {
        public List<Currency> currencies { get; set; }
    }

    public class CurrenciesResponseBody
    {
        public string base_currency { get; set; }
        public string quote_currency { get; set; }
        public DateTime close_time { get; set; }
        public string average_bid { get; set; }
        public string average_ask { get; set; }
        public string high_bid { get; set; }
        public string high_ask { get; set; }
        public string low_bid { get; set; }
        public string low_ask { get; set; }
    }

    public class CurrenciesResponse
    {
        public List<CurrenciesResponseBody> response { get; set; }
    }

}
