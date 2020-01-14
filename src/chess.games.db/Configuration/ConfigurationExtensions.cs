using System;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace chess.games.db.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Add a SQL based ChessGamesDbContext to the container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serverType"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddChessDatabaseContext(
            this IServiceCollection services,
            DbServerTypes serverType,
            string connectionString)
            => services.AddDbContext<ChessGamesDbContext>(
                opts =>
                {
                    switch (serverType)
                    {
                        case DbServerTypes.SQLServer:
                            opts.UseSqlServer(connectionString);
                            break;
                        case DbServerTypes.SQLite:
                            opts.UseSqlite(connectionString);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
                    }
                });

        // ReSharper disable InconsistentNaming
        public enum DbServerTypes
        {
            SQLServer,
            SQLite
        }
        // ReSharper restore InconsistentNaming
    }
}