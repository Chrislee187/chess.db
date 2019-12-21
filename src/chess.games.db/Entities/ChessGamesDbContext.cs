using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace chess.games.db.Entities
{
    public class ChessGamesDbContext : DbContext
    {
        public DbSet<PgnImportQueue> PgnImportQueue { get; set; }

//        public DbSet<Event> Events { get; set; }
//        public DbSet<Site> Sites { get; set; }
//        public DbSet<PgnPlayer> PgnPlayers { get; set; }
//        public DbSet<Player> Players { get; set; }
//        public DbSet<Game> Games { get; set; }
//        public DbSet<GameImport> GameImports { get; set; }

        public ChessGamesDbContext(DbContextOptions options) : base(options)
        {
            // NOTE: Used by .NET Core IoC/MVC Startup

        }

        public ChessGamesDbContext(string connectionString)
            : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PgnImportQueue>()
                .HasAlternateKey(p => new { p.Event, p.Site, p.White, p.Black, p.Date, p.Result, p.Round, p.MoveList })
                .HasName("XX")
                ;
//                .IsUnique();
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