using PriceAggregator.Integrations;

namespace PriceAggregator.Domain
{
    public interface IPriceCalculator
    {
        Task<decimal?> ConvertPrice(decimal price, string fromCurrency, string toCurrency);
    }

    public class PriceCalculator : IPriceCalculator
    {
        private readonly ICurrencyRatesAPI _currencyRatesAPI;

        public PriceCalculator(ICurrencyRatesAPI currencyRatesAPI)
        {
            {
                _currencyRatesAPI = currencyRatesAPI;
            }
        }
        public async Task<decimal?> ConvertPrice(decimal price, string fromCurrency, string toCurrency)
        {
            var rates = await _currencyRatesAPI.GetCurrencyRates();
            var fromCurrencyRate = rates.FirstOrDefault(x => x.code == fromCurrency);
            var toCurrencyRate = rates.FirstOrDefault(x => x.code == toCurrency);
            if (fromCurrencyRate == null || toCurrencyRate == null)
            {
                return null;
            }
            var lariPrice = (decimal)fromCurrencyRate.rate / fromCurrencyRate.quantity * price;
            var result = lariPrice / (decimal)toCurrencyRate.rate * toCurrencyRate.quantity;
            return result;
        }
    }
}
