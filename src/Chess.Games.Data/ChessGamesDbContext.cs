using Chess.Games.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chess.Games.Data
{
    public class ChessGamesDbContext : DbContext
    {
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<SiteEntity> Sites { get; set; }
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<GameEntity> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // var sqlServerDev = "Server=localhost;Database=ChessGames;Trusted_Connection=True;";
            // var sqlServerVS = "(localdb)\\v11.0;Integrated Security=true";
            // var sqlServerVS2 = "Data Source = (localdb)\\MSSQLLocalDb;Initial Catalog=ChessGames";
            var sqlServerDev2 = "Server=localhost;Database=ChessMatch;Trusted_Connection=True;Encrypt=false";
            optionsBuilder
                .UseSqlServer(sqlServerDev2)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameEntity>()
                .HasOne<PlayerEntity>(o => o.Black)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GameEntity>()
                .HasOne<PlayerEntity>(o => o.White)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GameEntity>()
                .HasOne<SiteEntity>(o => o.Site)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GameEntity>()
                .HasOne<EventEntity>(o => o.Event)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}