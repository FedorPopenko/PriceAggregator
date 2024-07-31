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
        private readonly ICurrencyDictionary _currencyDictionary;

        public GamesController(
            ILogger<GamesController> logger,
            GamesDB gamesDB,
            IPriceSearcher priceSearcher,
            ICurrencyDictionary currencyDictionary)
        {
            _logger = logger;
            _gamesDB = gamesDB;
            _priceSearcher = priceSearcher;
            _currencyDictionary = currencyDictionary;
        }

        [HttpGet("prices/{gameName}")]
        public async Task<Game> GetPrices(string gameName)
        {
            var searchPhrase = GetSearchPhrase(gameName);
            var gameDto = await _gamesDB.Games.Where(x => x.TitleIndex == searchPhrase).FirstOrDefaultAsync();
            var priceDtos = gameDto == null ?
                new() :
                await _gamesDB.Prices.Where(x => x.GameId == gameDto.Id).ToListAsync();

            if (gameDto == null || !priceDtos.Any())
            {
                var searchResult = await _priceSearcher.GetGamePrices(gameName.ToLower());
                if (searchResult == null || !searchResult.Any())
                {
                    return null;
                }
                if (gameDto == null)
                {
                    gameDto = new GameDTO
                    {
                        Title = searchResult.First().title,
                        TitleIndex = GetSearchPhrase(searchResult.First().title),
                    };
                    await _gamesDB.Games.AddAsync(gameDto);
                    await _gamesDB.SaveChangesAsync();
                }

                foreach (var price in searchResult)
                {
                    var currency = _currencyDictionary.GetCurrency(price.country);
                    priceDtos.Add(new PriceDTO
                    {
                        Price = price.price,
                        Country = price.country,
                        Currency = currency,
                        GameId = gameDto.Id
                    });
                }
                await _gamesDB.Prices.AddRangeAsync(priceDtos);
                await _gamesDB.SaveChangesAsync();
            }
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
