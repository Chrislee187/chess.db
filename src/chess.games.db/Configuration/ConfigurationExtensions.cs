using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace chess.games.db.Configuration
{
    public static class ConfigurationExtensions
    {
        private static readonly string DefaultSQLiteFile = "ChessDB.sqlite";
        private static readonly string DefaultDataFolder = "ChessDB";

        public static Action<string> Reporter = Console.WriteLine;
        /// <summary>
        /// Add a SQL based ChessGamesDbContext to the container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serverType"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddChessDatabaseContext(
            this IServiceCollection services,
            DbServerTypes serverType,
            string connectionString)
            => services.AddDbContext<ChessGamesDbContext>(
                opts =>
                {
                    switch (serverType)
                    {
                        case DbServerTypes.SQLServer:
                            opts.UseSqlServer(connectionString);
                            break;
                        case DbServerTypes.SQLite:
                            opts.UseSqlite(connectionString);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
                    }
                });

        public static IConfiguration Configuration(string[] args = null) 
            => BaseConfigBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile()
            .AddCommandLine(args)
            .Build();

        public static DbServerTypes ServerType(this IConfiguration config)
        {
            var serverType = config["DbServerType"];

            if (string.IsNullOrWhiteSpace(serverType))
            {
                // TODO: Log this default DB type is being used
                // Reporter("No DB type specified, using SQLite.");
                return DbServerTypes.SQLite;
            }
            try
            {
                return Enum.Parse<DbServerTypes>(serverType);
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException($"Invalid DB ServerType '{serverType}'", e);
            }
        }

        public static string ConnectionString(this IConfiguration config)
        {
            var connectionString = config["ChessDB"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                if (config.ServerType() == DbServerTypes.SQLite)
                {
                    // TODO: Log this default data source is being used
                    var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var dataFolder = Path.Combine(myDocs, DefaultDataFolder);
                    var combine = Path.Combine(dataFolder, DefaultSQLiteFile);
                    if (!Directory.Exists(dataFolder))
                    {
                        Directory.CreateDirectory(dataFolder);
                    }
                    
                    if (!File.Exists(combine))
                    {
                        Reporter($"Creating new SQLite database: {combine}");
                    }

                    connectionString = $"Data Source={combine}";
                }
                else
                {
                    throw new ApplicationException("SQL Server connection string cannot be defaulted!");
                }
            }
            return connectionString;
        }

        public static async Task<ChessGamesDbContext> InitDb(string[] args = null)
        {
            var config = Configuration(args);

            var dbType = config.ServerType();
            Reporter($"Connecting to {dbType} chess database...");
            var dbContext = config.CreateDbContext(dbType);

            Reporter("  Checking for pending migrations...");

            var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
            if (pendingMigrations.Any())
            {
                Reporter($"  Applying {pendingMigrations.Count} pending migration(s)...");
                dbContext.Database.Migrate();
                Reporter("  ... database successfully updated.");
            }
            else
            {
                Reporter("  No pending migrations");
            }

            return dbContext;
        }

        private static ChessGamesDbContext CreateDbContext(this IConfiguration config, 
            DbServerTypes? dbType = null, 
            string connString = null)
        {
            var serverType = dbType ?? config.ServerType();
            var connectionString = connString ?? config.ConnectionString();

            return new ChessGamesDbContext(serverType, connectionString);
        }

        // ReSharper disable InconsistentNaming
        public enum DbServerTypes
        {
            SQLServer,
            SQLite
        }
        // ReSharper restore InconsistentNaming
        private static IConfigurationBuilder BaseConfigBuilder()
            => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

        private static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder)
            => builder.AddJsonFile("appSettings.json", optional: false);

        private static IConfigurationBuilder AddEnvironmentVariables(this IConfigurationBuilder builder)
            => builder; // TODO: Add env var support

        private static IConfigurationBuilder AddCommandLine(this IConfigurationBuilder builder, string[] args = null)
            => args == null ? builder : builder.AddCommandLine(args);

    }

}