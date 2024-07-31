using Microsoft.EntityFrameworkCore;

namespace PriceAggregator.DAL
{
    public class GamesDB : DbContext
    {
        public DbSet<GameDTO> Games { get; set; }
        public DbSet<PriceDTO> Prices { get; set; }

        public GamesDB(DbContextOptions<GamesDB> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PriceDTO>()
                .HasOne<GameDTO>()
                .WithMany()
                .HasForeignKey(f => f.GameId);
        }

    }

    public class PriceDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; }
        public string Country { get; set; }
        public int GameId { get; set; }
    }

    public class GameDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleIndex { get; set; }
    }
}
