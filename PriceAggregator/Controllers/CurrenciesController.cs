using Microsoft.AspNetCore.Mvc;
using PriceAggregator.Integrations;
using System.Globalization;

namespace PriceAggregator.Controllers
{
    [ApiController]
    [Route("currencies")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ILogger<CurrenciesController> _logger;
        private readonly ICurrencyRatesAPI _currencyRatesAPI;
        private readonly Dictionary<string, string> _currenciesToCurrencyTag = new()
        {
            { "$", "USD" },
            { "€", "EUR" },
        };

        public CurrenciesController(
            ILogger<CurrenciesController> logger,
            ICurrencyRatesAPI currencyRatesAPI)
        {
            _logger = logger;
            _currencyRatesAPI = currencyRatesAPI;
        }

        [HttpGet("currentRate")]
        public async Task<float> Get(string baseCurrency, string quoteCurrency)
        {
            _currenciesToCurrencyTag.TryGetValue(baseCurrency, out var baseCurrencyTag);
            _currenciesToCurrencyTag.TryGetValue(quoteCurrency, out var quoteCurrencyTag);
            var rate = await _currencyRatesAPI.GetCurrencyRate(baseCurrencyTag, quoteCurrencyTag, DateTime.Today.AddDays(-1), DateTime.Today);
            return float.Parse(rate, CultureInfo.InvariantCulture);
        }
    }
}