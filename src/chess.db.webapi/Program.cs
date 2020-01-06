using System;
using System.IO;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;

namespace chess.db.webapi
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        public static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
//                .WriteTo.File(new JsonFormatter(), @"D\logs\chess.db.webapi.log.json", shared: true)
                .CreateLogger();

            var host = CreateHostBuilder(args)
                .Build();
            
            using (var scoped = host.Services.CreateScope())
            {
                try
                {
                    Log.Information("Migrating database to latest version.");
                    var context = scoped.ServiceProvider.GetService<ChessGamesDbContext>();
                    // Ensure DB is is in-sync with code
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    Log.Fatal(e, "An error occured while migrating the database.");
                    throw;
                }
            }

            try
            {
                Log.Information("Starting web host");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Web host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseSerilog();
                });
    }
}
