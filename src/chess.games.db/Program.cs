using System;
using System.Linq;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;

namespace chess.games.db
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            Console.WriteLine("Chess DB");
            var c = new ChessGamesDbContext(@"Server=.\Dev;Database=ChessGames;Trusted_Connection=True;");
            if (c.Database.GetPendingMigrations().Any())
            {
                Console.WriteLine("Applying pending migrations...");
                c.Database.Migrate();
            }
        }
    }


}
