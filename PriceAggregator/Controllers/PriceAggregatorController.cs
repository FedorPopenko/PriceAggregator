using Microsoft.AspNetCore.Mvc;
using PriceAggregator.DAL;
using PriceAggregator.Integrations;
using System.Reflection;

namespace PriceAggregator.Controllers
{
    [ApiController]
    [Route("games-prices")]
    public class PriceAggregatorController : ControllerBase
    {
        private readonly ILogger<PriceAggregatorController> _logger;
        private readonly GamesDB _gamesDB;
        private readonly IPriceSearcher _priceSearcher;

        public PriceAggregatorController(
            ILogger<PriceAggregatorController> logger,
            GamesDB gamesDB,
            IPriceSearcher priceSearcher)
        {
            _logger = logger;
            _gamesDB = gamesDB;
            _priceSearcher = priceSearcher;
        }

        [HttpGet("{gameName}")]
        public Game Get(string gameName)
        {
            var searchPhrase = GetSearchPhrase(gameName);
            var gameDto = _gamesDB.Games.Where(x => x.TitleIndex == searchPhrase).FirstOrDefault();
            if (gameDto == null)
            {
                var searchResult = _priceSearcher.GetGamePrices(gameName);
                if(searchResult == null || !searchResult.Any())
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
                        Price = 5051,//price.price,
                        Country = price.country,
                        Currency ="sosi",
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
