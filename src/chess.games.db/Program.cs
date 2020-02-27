using System;
using System.Linq;
using System.Threading.Tasks;
using chess.games.db.Configuration;
using Serilog;

namespace chess.games.db
{
    class Program
    {
        private static ILogger _logger;
        private static Action<string> Reporter = (text) =>
        {
            Console.WriteLine(text);
            _logger.Information(text);
        };

        // ReSharper disable once UnusedParameter.Local
        private static async Task Main(string[] args)
        {
            _logger = new LoggerConfiguration()
                .ReadFrom.Configuration(ConfigurationExtensions.Configuration()).CreateLogger();
            Log.Logger = _logger;

            DbStartup.Reporter = Reporter;

            Reporter($"Chess DB Creator");

            var dbContext = await DbStartup.InitDbAsync();

            Reporter("Chess DB Status");
            Reporter($"  Valid games: {dbContext.Games.Count()}");
            Reporter($"  Pending validation: {dbContext.PgnImports.Count()}"); 
            Reporter($"  Failed validations: {dbContext.PgnImportErrors.Count()}");
        }
    }


}
