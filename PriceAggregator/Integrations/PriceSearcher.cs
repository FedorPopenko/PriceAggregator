namespace PriceAggregator.Integrations
{
    public interface IPriceSearcher
    {
        List<GamePriceDTO> GetGamePrices(string name);
    }

    public class PriceSearcher : IPriceSearcher
    {
        public List<GamePriceDTO> GetGamePrices(string name)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(600);
            client.BaseAddress = new Uri($"http://127.0.0.1:8000/show_table/{name}");
            var response = client.GetFromJsonAsync<List<GamePriceDTO>>(name).Result;
            return response;
        }
    }

    public class GamePriceDTO
    {
        public string country { get; set; }
        public string title { get; set; }
        public decimal price { get; set; }
        public string currency { get; set; }
        public string publisher { get; set; }
    }
}
