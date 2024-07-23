namespace PriceAggregator.Integrations
{
    public interface IPriceSearcher
    {
        Task<List<GamePriceDTO>> GetGamePrices(string name);
        Task<List<string>> GetPS4GameTitles();
    }

    public class PriceSearcher : IPriceSearcher
    {
        public async Task<List<GamePriceDTO>> GetGamePrices(string name)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(600);
            client.BaseAddress = new Uri($"http://127.0.0.1:8000");
            var response = await client.GetFromJsonAsync<List<GamePriceDTO>>($"/country-prices/{name}");
            return response;
        }
        public async Task<List<string>> GetPS4GameTitles()
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(600);
            client.BaseAddress = new Uri($"http://127.0.0.1:8000");
            var response = await client.GetFromJsonAsync<List<string>>($"ps4-names");
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
