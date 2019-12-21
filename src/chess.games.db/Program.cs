using System;
using System.Linq;
using System.Threading.Tasks;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace chess.games.db
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, false)
                .Build();

            var connectionString = config["chess-games-db"];

            Console.WriteLine("Connecting to DB...");
            var c = new ChessGamesDbContext(connectionString);

            Console.WriteLine("  Checking for pending migrations...");
            if (c.Database.GetPendingMigrations().Any())
            {
                var ms = await c.Database.GetPendingMigrationsAsync();
                Console.WriteLine($"Applying {ms.Count()} pending migration(s)...");
                
                c.Database.Migrate();
                Console.WriteLine("... database successfully updated.");
            }
        }
    }


}
