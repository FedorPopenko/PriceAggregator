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
            { "Argentina", "USD" },
            { "Bolivia", "USD" },
            { "Brazil", "BRL" },
            { "Canada", "CAD" },
            { "Chile", "USD" },
            { "Colombia", "USD" },
            { "Costa Rica", "USD" },
            { "Ecuador", "USD" },
            { "El Salvador", "USD" },
            { "Guatemala", "USD" },
            { "Honduras", "USD" },
            { "Mexico", "USD" },
            { "Nicaragua", "USD" },
            { "Panama", "USD" },
            { "Paraguay", "USD" },
            { "Peru", "USD" },
            { "United States", "USD" },
            { "Uruguay", "USD" },
            { "Australia", "AUD" },
            { "Austria", "EUR" },
            { "Bahrain", "USD" },
            { "Belgium", "EUR" },
            { "Bulgaria", "BGN" },
            { "Croatia", "EUR" },
            { "Cyprus", "EUR" },
            { "Czech Republic", "CZK" },
            { "Denmark", "DKK" },
            { "Finland", "EUR" },
            { "France", "EUR" },
            { "Germany", "EUR" },
            { "Greece", "EUR" },
            { "Hungary", "HUF" },
            { "Iceland", "EUR" },
            { "India", "INR" },
            { "Ireland", "EUR" },
            { "Israel", "ILS" },
            { "Italy", "EUR" },
            { "Kuwait", "USD" },
            { "Lebanon", "USD" },
            { "Luxembourg", "EUR" },
            { "Malta", "EUR" },
            { "Netherlands", "EUR" },
            { "New Zealand", "NZD" },
            { "Norway", "NOK" },
            { "Oman", "USD" },
            { "Poland", "PLN" },
            { "Portugal", "EUR" },
            { "Qatar", "USD" },
            { "Romania", "RON" },
            { "Russia", "RUB" },
            { "Saudi Arabia", "USD" },
            { "Serbia", "RSD" },
            { "Slovakia", "EUR" },
            { "Slovenia", "EUR" },
            { "South Africa", "ZAR" },
            { "Spain", "EUR" },
            { "Sweden", "SEK" },
            { "Switzerland", "CHF" },
            { "Turkey", "TRY" },
            { "Ukraine", "UAH" },
            { "United Arab Emirates", "USD" },
            { "United Kingdom", "GBP" },
            { "Hong Kong", "HKD" },
            { "Indonesia", "IDR" },
            { "Japan", "JPY" },
            { "Mainland China", "CNY" },
            { "Malaysia", "MYR" },
            { "Philippines", "PHP" },
            { "Singapore", "SGD" },
            { "South Korea", "KRW" },
            { "Taiwan", "TWD" },
            { "Thailand", "THB" },
            { "Vietnam", "VND" },
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
