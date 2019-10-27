using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace chess.games.db
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Add a SQL based ChessGamesDbContext to the container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddChessDatabaseContext(this IServiceCollection services, string connectionString) 
            => services.AddDbContext<ChessGamesDbContext>(
                opts => opts.UseSqlServer(connectionString)
            );
    }
}