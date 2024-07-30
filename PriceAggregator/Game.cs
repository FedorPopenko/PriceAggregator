namespace PriceAggregator
{
    public class Game
    {
        public string Title { get; set; }
        public IEnumerable<GamePrice> Prices { get; set; }
    }

    public class GamePrice
    {
        public string Country { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; }
    }
}
