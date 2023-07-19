using Chess.Data.PGNImporter;
using chess.engine.Game;
using Chess.Games.Data;
using Chess.Games.Data.Repos;
using Chess.Games.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        AddConfig(services);

        AddLogging(services);

        services.AddDbContext<ChessGamesDbContext>(opts =>
        {
            opts.UseSqlServer("name=Database:ChessMatches:ConnectionString");
        });

        AddRepos(services);

        AddServices(services);

        services.AddSingleton<Startup>();

    })
    .Build();

MigrateDatabase();

var app = host.Services.GetRequiredService<Startup>();
app.Execute(args);

void AddLogging(IServiceCollection serviceCollection)
{
    serviceCollection.AddLogging(bld =>
    {
        bld.AddSimpleConsole(o =>
        {
            o.SingleLine = true;
            o.UseUtcTimestamp = true;
            o.IncludeScopes = true;
            o.TimestampFormat = "HH:mm:ss:ffffff ";
        });
        bld.AddDebug();
    });
}

void AddConfig(IServiceCollection serviceCollection)
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        // TODO: Add env specific settings support
        .Build();

    serviceCollection.AddSingleton(configuration);
}

void AddRepos(IServiceCollection serviceCollection)
{
    serviceCollection.AddScoped<IEventRepository, EventRepo>();
    serviceCollection.AddScoped<ISiteRepository, SiteRepo>();
    serviceCollection.AddScoped<IPlayerRepository, PlayerRepo>();
    serviceCollection.AddScoped<IGameRepository, GameRepo>();
}

void AddServices(IServiceCollection serviceCollection)
{
    serviceCollection.AddScoped<IEventIndexingService, EventIndexingService>();
    serviceCollection.AddScoped<ISiteIndexingService, SiteIndexingService>();
    serviceCollection.AddScoped<IPlayerIndexingService, PlayerIndexingService>();
    serviceCollection.AddScoped<IGameIndexingService, GameIndexingService>();
    serviceCollection.AddScoped<IImporter, Importer>();
    serviceCollection.AddScoped<IChessBoardStateSerializer, ChessBoardStateSerializer>();

}

void MigrateDatabase()
{
    var dbContext = host.Services.GetRequiredService<ChessGamesDbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.Database.Migrate();
}

