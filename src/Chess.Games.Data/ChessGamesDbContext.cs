using Chess.Games.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Chess.Games.Data
{
    public class ChessGamesDbContext : DbContext
    {
        private readonly bool _enableLogging;
        private readonly bool _insensitiveLogging;
        private readonly ILogger<ChessGamesDbContext> _logger;

        public virtual DbSet<EventEntity>? Events { get; set; }
        public virtual DbSet<SiteEntity>? Sites { get; set; }
        public virtual DbSet<PlayerEntity>? Players { get; set; }
        public virtual DbSet<GameEntity>? Games { get; set; }

        private readonly string _conString = "Server=localhost;Database=ChessMatch;Trusted_Connection=True;Encrypt=false;";

        public ChessGamesDbContext()
        {
            _logger = NullLogger<ChessGamesDbContext>.Instance;
        }
        public ChessGamesDbContext(DbContextOptions<ChessGamesDbContext> options, ILogger<ChessGamesDbContext>? logger = null) : base(options)
        {
            _logger ??= NullLogger<ChessGamesDbContext>.Instance;
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
                _enableLogging = enableLogging;
            }
            // TODO: Should only be enabled on DEV envs.
            if (bool.TryParse(config["Database:ChessMatches:InsensitiveLogging"], out bool insensitiveLogging))
            {
                _insensitiveLogging = insensitiveLogging;
            }
            _conString = "name=Database:ChessMatches:ConnectionString";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(_conString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            }

            if (_enableLogging)
            {
                optionsBuilder.LogTo((m) => _logger.LogInformation(m),
                    new[] { DbLoggerCategory.Database.Command.Name });
            }

            if (_insensitiveLogging)
            {
                optionsBuilder.EnableSensitiveDataLogging(_insensitiveLogging);
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