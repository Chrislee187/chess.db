using Chess.Data.PGNImporter;
using Chess.Games.Data;
using Chess.Games.Data.Repos;
using Chess.Games.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var _host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        AddLogging(services);

        AddConfig(services);

        services.AddScoped<DbContext, ChessGamesDbContext>();

        AddRepos(services);

        AddServices(services);

        services.AddSingleton<Startup>();

    })
    .Build();

UpdateDatabase();

var app = _host.Services.GetRequiredService<Startup>();
app.Execute();

void AddLogging(IServiceCollection serviceCollection)
{
    serviceCollection.AddLogging(bld =>
    {
        bld.AddConsole();
        bld.AddDebug();
    });
}

void AddConfig(IServiceCollection serviceCollection)
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
}

void UpdateDatabase()
{
    var dbContext = _host.Services.GetRequiredService<DbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.Database.Migrate();
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
    serviceCollection.AddSingleton<IEventIndexingService, EventIndexingService>();
    serviceCollection.AddSingleton<ISiteIndexingService, SiteIndexingService>();
    serviceCollection.AddSingleton<IPlayerIndexingService, PlayerIndexingService>();
    serviceCollection.AddSingleton<IGameIndexingService, GameIndexingService>();
}

