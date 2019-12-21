using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace chess.games.db.Entities
{
    // NOTE: This is only used at design time when using update-database.
    // used for migrations that take longer than the default 30 sec timeout
    public class ChessGamesDbContextFactory : IDesignTimeDbContextFactory<ChessGamesDbContext>
    {
        public ChessGamesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChessGamesDbContext>();
            
            optionsBuilder.UseSqlServer(@"Server=.\Dev;Database=ChessGames;Trusted_Connection=True;",
                opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));

            return new ChessGamesDbContext(optionsBuilder.Options);
        }

    }
}