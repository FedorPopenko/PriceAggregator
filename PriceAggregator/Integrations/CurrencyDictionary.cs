namespace PriceAggregator.Integrations
{
    public interface ICurrencyDictionary
    {
        string? GetCurrency(string country);
    }

    public class CurrencyDictionary : ICurrencyDictionary
    {
        private readonly ILogger<CurrencyDictionary> _logger;
        private readonly Dictionary<string, string> _countriesToCurrencies = new()
        {
            { "United Kingdom", "GBP" },
            { "United States", "USD" },
            { "Austria", "EUR"},
        };
        public CurrencyDictionary(ILogger<CurrencyDictionary> logger)
        {
            _logger = logger;
        }
        public string? GetCurrency(string country)
        {
            if (_countriesToCurrencies.TryGetValue(country, out var currency))
                return currency;
            _logger.LogError($"Not found country {country}");
            return null;
        }
    }
}
