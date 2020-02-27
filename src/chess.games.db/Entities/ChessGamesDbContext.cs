using System;
using chess.games.db.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace chess.games.db.Entities
{
    public class ChessGamesDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
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
            DbServerTypes serverType,
            string connectionString, 
            ILoggerFactory loggerFactory = null)
            : base(new DbContextOptionsBuilder()
                .UseSql(serverType, connectionString)
                .Options)
        {
            _loggerFactory = loggerFactory;
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
    }
}