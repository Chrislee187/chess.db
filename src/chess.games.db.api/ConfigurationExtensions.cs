using chess.games.db.api.Players;
using Microsoft.Extensions.DependencyInjection;

namespace chess.games.db.api
{
    public static class ConfigurationExtensions
    {
        public static void AddChessRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPlayersRepository, PlayersRepository>();
            services.AddScoped<IPgnPlayersRepository, PgnPlayersRepository>();
        }
    }
}