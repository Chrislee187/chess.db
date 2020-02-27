using Microsoft.Extensions.Configuration;
using System.IO;

namespace chess.games.db.Configuration
{
    public static class ConfigurationExtensions
    {


        public static IConfiguration Configuration(string[] args = null) 
            => BaseConfigBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile()
                .AddCommandLineSafe(args)
                .Build();



        // ReSharper disable InconsistentNaming
        // ReSharper restore InconsistentNaming
        private static IConfigurationBuilder BaseConfigBuilder()
            => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

        private static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder)
            => builder.AddJsonFile("appSettings.json", optional: false);

        private static IConfigurationBuilder AddEnvironmentVariables(this IConfigurationBuilder builder)
            => builder; // TODO: Add env var support

        private static IConfigurationBuilder AddCommandLineSafe(this IConfigurationBuilder builder, string[] args = null)
            => args == null ? builder : builder.AddCommandLine(args);
    }
}