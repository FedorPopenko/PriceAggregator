using Microsoft.AspNetCore.Mvc;
using PriceAggregator.DAL;

namespace PriceAggregator.Controllers
{
    [ApiController]
    [Route("games-prices")]
    public class PriceAggregatorController : ControllerBase
    {
        private static readonly Game _game =
            new Game
            {
                Title = "Game of War",
                Prices = [
                    new GamePrice
                    {
                        Country = "USA",
                        Price = 15,
                        Currency = "$"
                    }
                ]
            };

        private readonly ILogger<PriceAggregatorController> _logger;
        private readonly GamesDB _gamesDB;

        public PriceAggregatorController(
            ILogger<PriceAggregatorController> logger,
            GamesDB gamesDB)
        {
            _logger = logger;
            _gamesDB = gamesDB;
        }

        [HttpGet("{gameName}")]
        public Game Get(string gameName)
        {
            var searchPhrase = GetSearchPhrase(gameName);
            var gameDto = _gamesDB.Games.Where(x => x.TitleIndex == searchPhrase).FirstOrDefault();
            if (gameDto == null)
            {
                return null;
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