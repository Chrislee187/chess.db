using System;
using System.Threading.Tasks;
using chess.games.db.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using ConfigurationExtensions = chess.games.db.Configuration.ConfigurationExtensions;

namespace chess.db.webapi
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(ConfigurationExtensions.Configuration())
                .CreateLogger();
            
            var host = CreateHostBuilder(args)
                .Build();
            
            try
            {
                Log.Information("Migrating database to latest version.");
                DbStartup.Reporter = Log.Information;
                await DbStartup.InitDbAsync(args);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "An error occured while migrating the database.");
                throw;
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
                Log.Information("Shutting down.");
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
