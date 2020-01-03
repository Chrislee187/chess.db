using chess.games.db.api.PgnPlayers;
using chess.games.db.api.Players;
using Microsoft.Extensions.DependencyInjection;

namespace chess.games.db.api
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Add Chess DB Repositories to the container
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddChessRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPlayersRepository, PlayersRepository>();
            services.AddScoped<IGamesRepository, GamesRepository>();
            services.AddScoped<ISitesRepository, SitesRepository>();
            services.AddScoped<IEventsRepository, EventsRepository>();

            return services;
        }
    }
}