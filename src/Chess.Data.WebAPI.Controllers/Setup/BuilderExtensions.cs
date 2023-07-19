using Chess.Games.Data;
using EasyEF.Repos;
using Microsoft.EntityFrameworkCore;

namespace Chess.Data.WebAPI.Controllers.Setup;

public static class BuilderExtensions
{
    public static void AddChessGameDbAndRepositories(this IServiceCollection serviceCollection)
    {
        // Add a concrete DbContext
        serviceCollection.AddDbContext<ChessGamesDbContext>(opts =>
        {
            opts.UseSqlServer("name=Database:ChessMatches:ConnectionString");
        });
        // Add the more generic DbContext to reference our concrete context
        serviceCollection.AddScoped(s => (s.GetService(typeof(ChessGamesDbContext)) as DbContext)!);

        // Add generic default LINQ repo support for entities
        serviceCollection.AddScoped(typeof(ILinqRepository<>), typeof(EasyEfRepository<>));
    }
}