using Chess.Games.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Chess.Games.Data
{
    public class ChessGamesDbContext : DbContext
    {
        public static bool EnableLogging;
        private IConfigurationRoot _config;
        private ILogger<ChessGamesDbContext> _logger;

        public virtual DbSet<EventEntity> Events { get; set; }
        public virtual DbSet<SiteEntity> Sites { get; set; }
        public virtual DbSet<PlayerEntity> Players { get; set; }
        public virtual DbSet<GameEntity> Games { get; set; }

        public ChessGamesDbContext()
        {
            _logger = NullLogger<ChessGamesDbContext>.Instance;
        }

        public ChessGamesDbContext(
            DbContextOptions<ChessGamesDbContext> options, 
            IConfigurationRoot config,
            ILogger<ChessGamesDbContext> logger)
            : base(options)
        {
            _logger = logger;
            if(bool.TryParse(config["Database:ChessMatches:EfLogging"], out bool enableLogging))
            {
                EnableLogging = enableLogging;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqlServerDev2 = "name=Database:ChessMatches:ConnectionString";
            optionsBuilder
                .UseSqlServer(sqlServerDev2)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

            if (EnableLogging)
            {
                optionsBuilder.LogTo((m) => _logger.LogInformation(m), new [] {DbLoggerCategory.Migrations.Name});
            }
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