using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace chess.games.db
{
    public static class ConfigurationExtensions
    {
        public static void AddChessDatabaseContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ChessGamesDbContext>(
                opts => opts.UseSqlServer(connectionString)
            );
        }
    }
}