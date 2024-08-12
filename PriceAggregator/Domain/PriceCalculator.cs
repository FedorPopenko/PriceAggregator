namespace PriceAggregator.Domain
{
    public interface IPriceCalculator
    {
        decimal ConvertPrice(decimal price, string fromCurrency, string toCurrency);
    }

    public class PriceCalculator : IPriceCalculator
    {
        public PriceCalculator()
        {
            
        }
        public decimal ConvertPrice(decimal price, string fromCurrency, string toCurrency)
        {
            var rate = new CurrencyRate
            {
                Quantity = 10,
                Rate = 1.7M,
            };
            var result = rate.Rate / rate.Quantity * price;
            return result;
        }
    }
    public class CurrencyRate
    {
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
