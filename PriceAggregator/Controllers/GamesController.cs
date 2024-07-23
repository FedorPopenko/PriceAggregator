using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceAggregator.DAL;
using PriceAggregator.Integrations;

namespace PriceAggregator.Controllers
{
    [ApiController]
    [Route("games")]
    public class GamesController : ControllerBase
    {
        private readonly ILogger<GamesController> _logger;
        private readonly GamesDB _gamesDB;
        private readonly IPriceSearcher _priceSearcher;

        public GamesController(
            ILogger<GamesController> logger,
            GamesDB gamesDB,
            IPriceSearcher priceSearcher)
        {
            _logger = logger;
            _gamesDB = gamesDB;
            _priceSearcher = priceSearcher;
        }

        [HttpGet("prices/{gameName}")]
        public async Task<Game> GetPrices(string gameName)
        {
            var searchPhrase = GetSearchPhrase(gameName);
            var gameDto = _gamesDB.Games.Where(x => x.TitleIndex == searchPhrase).FirstOrDefault();
            if (gameDto == null)
            {
                var searchResult = await _priceSearcher.GetGamePrices(gameName);
                if (searchResult == null || !searchResult.Any())
                {
                    return null;
                }
                gameDto = new GameDTO
                {
                    Title = searchResult.First().title,
                    TitleIndex = GetSearchPhrase(searchResult.First().title),
                };
                _gamesDB.Games.Add(gameDto);
                _gamesDB.SaveChanges();
                foreach (var price in searchResult)
                {
                    _gamesDB.Prices.Add(new PriceDTO
                    {
                        Price = price.price,
                        Country = price.country,
                        Currency = price.currency,
                        GameId = gameDto.Id
                    });
                }
                _gamesDB.SaveChanges();

            }
            var priceDtos = _gamesDB.Prices.Where(x => x.GameId == gameDto.Id).ToList();
            return new Game
            {
                Title = gameDto.Title,
                Prices = priceDtos.Select(x => new GamePrice
                {
                    Country = x.Country,
                    Price = x.Price,
                    Currency = x.Currency
                })
            };
        }
        [HttpGet("titles/{searchPhrase}")]
        public async Task<List<string>> GetTitles(string searchPhrase)
        {
            var searchIndex = GetSearchPhrase(searchPhrase);
            var gamesDto = await _gamesDB.Games.Where(g =>
                g.TitleIndex.Contains(searchIndex))
                .ToListAsync();
            var titles = gamesDto.Select(g => g.Title).ToList();
            return titles;
        }
        [HttpPost()]
        public void Get(Game game)
        {
            var gameDto = _gamesDB.Games.Add(new GameDTO
            {
                Title = game.Title,
                TitleIndex = GetSearchPhrase(game.Title),
            });
            _gamesDB.SaveChanges();
            foreach (var price in game.Prices)
            {
                _gamesDB.Prices.Add(new PriceDTO
                {
                    Price = price.Price,
                    Country = price.Country,
                    Currency = price.Currency,
                    GameId = gameDto.Entity.Id
                });
            }
            _gamesDB.SaveChanges();
        }

        private static string GetSearchPhrase(string gameName)
        {
            return gameName.ToLower().Replace(" ", string.Empty);
        }
    }
}
