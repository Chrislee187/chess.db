using System;
using System.Linq;
using chess.games.db.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace chess.games.db.Entities
{
    public class ChessGamesDbContext : DbContext
    {
        private ILoggerFactory _loggerFactory;
        public DbSet<PgnGame> PgnGames { get; set; }
        public DbSet<PgnImport> PgnImports { get; set; }

        public DbSet<PgnSite> SiteLookup { get; set; }
        public DbSet<PgnPlayer> PlayerLookup { get; set; }
        public DbSet<PgnEvent> EventLookup { get; set; }

        public DbSet<Site> Sites { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Player> Players { get; set; }

        public DbSet<Game> Games { get; set; }
        public DbSet<ImportedPgnGameIds> ImportedPgnGameIds { get; set; }
        public DbSet<PgnImportError> PgnImportErrors { get; set; }

        public ChessGamesDbContext(DbContextOptions options) : base(options)
        {
            // NOTE: Used by .NET Core IoC/MVC Startup
        }

        public ChessGamesDbContext(
            ConfigurationExtensions.DbServerTypes serverType,
            string connectionString, 
            ILoggerFactory loggerFactory = null)
            : base(new DbContextOptionsBuilder()
                .UseSql(serverType, connectionString)
                .Options)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
        public void RunWithExtendedTimeout(Action action, TimeSpan timeout)
        {
            var oldTimeOut = Database.GetCommandTimeout();
            Database.SetCommandTimeout(timeout);

            action();

            Database.SetCommandTimeout(oldTimeOut);
        }

        public void UpdateDatabase()
        {
            var migs = Database.GetPendingMigrations().ToList();

            if (migs.Any())
            {
                Console.WriteLine("Pending DB migrations:");
                migs.ForEach(m => Console.WriteLine($"  {m}"));

                Console.WriteLine("Applying...");
                var oldTimeOut = Database.GetCommandTimeout();
                Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

                Database.Migrate();
                Database.SetCommandTimeout(oldTimeOut);
                Console.WriteLine("DB Migrated");
            }
        }

    }
}