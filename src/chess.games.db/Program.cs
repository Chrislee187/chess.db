using System;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationExtensions = chess.games.db.Configuration.ConfigurationExtensions;

namespace chess.games.db
{
    class Program
    {
        private static Action<string> Reporter = Console.WriteLine;

        // ReSharper disable once UnusedParameter.Local
        private static async Task Main(string[] args)
        {
            ConfigurationExtensions.Reporter = Reporter;

            Reporter($"Chess DB Creator");

            var dbContext = await ConfigurationExtensions.InitDb();

            Reporter("Chess DB Status");
            Reporter($"  Valid games: {dbContext.Games.Count()}");
            Reporter($"  Pending validation: {dbContext.PgnImports.Count()}"); 
            Reporter($"  Failed validations: {dbContext.PgnImportErrors.Count()}");
        }
    }


}
