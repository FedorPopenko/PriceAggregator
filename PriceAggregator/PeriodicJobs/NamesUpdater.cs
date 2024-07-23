
using Microsoft.EntityFrameworkCore;
using PriceAggregator.DAL;
using PriceAggregator.Integrations;

namespace PriceAggregator.PeriodicJobs
{
    public class NamesUpdater : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<NamesUpdater> logger;

        public NamesUpdater(
            IServiceProvider serviceProvider,
            ILogger<NamesUpdater> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            var gamesDB = scope.ServiceProvider.GetRequiredService<GamesDB>();
            var priceSearcher = scope.ServiceProvider.GetRequiredService<IPriceSearcher>();
            do
            {
                try
                {
                    var titles = await priceSearcher.GetPS4GameTitles();
                    var newDtos = new List<GameDTO>();
                    foreach (var title in titles)
                    {
                        var titleExist = await gamesDB.Games.AnyAsync(g => g.Title == title);
                        if (!titleExist)
                            newDtos.Add(new GameDTO
                            {
                                Title = title,
                                TitleIndex = GetSearchPhrase(title),
                            });
                    }
                    await gamesDB.Games.AddRangeAsync(newDtos);
                    await gamesDB.SaveChangesAsync();
                    var timeUntillMidnight = DateTime.Today.AddDays(1) - DateTime.Now;
                    logger.LogInformation("Titles are updated");
                    await Task.Delay((int)timeUntillMidnight.TotalMilliseconds, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error during titles update {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            } while (!stoppingToken.IsCancellationRequested);
        }
        private static string GetSearchPhrase(string gameName)
        {
            return gameName.ToLower().Replace(" ", string.Empty);
        }
    }
}
