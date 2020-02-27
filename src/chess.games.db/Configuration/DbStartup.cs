using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace chess.games.db.Configuration
{
    public static class DbStartup
    {
        private static readonly string DefaultSQLiteFile = "ChessDB.sqlite";
        private static readonly string DefaultDataFolder = "ChessDB";

        public static Action<string> Reporter = (_) => { };

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
        {
            return services.AddDbContext<ChessGamesDbContext>(
                opts =>
                {
                    switch (serverType)
                    {
                        case DbServerTypes.SqlServer:
                            opts.UseSqlServer(connectionString);
                            break;
                        case DbServerTypes.Sqlite:
                            opts.UseSqlite(connectionString);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
                    }
                });
        }


        public static DbServerTypes ServerType(this IConfiguration config)
        {
            var serverType = config["DbServerType"];

            if (String.IsNullOrWhiteSpace(serverType))
            {
                // TODO: Log this default DB type is being used
                // Reporter("No DB type specified, using SQLite.");
                return DbServerTypes.Sqlite;
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
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                if (config.ServerType() == DbServerTypes.Sqlite)
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

        public static async Task<ChessGamesDbContext> InitDbAsync(string[] args = null, ILoggerFactory loggerFactory = null)
        {
            var config = ConfigurationExtensions.Configuration(args);

            var dbType = config.ServerType();
            var dbContext = config.CreateDbContext(dbType, loggerFactory: loggerFactory);

            Reporter($"Connecting to {dbType} chess database...");
            Reporter("  Checking for pending migrations...");

            var pendingMigrations = (await RelationalDatabaseFacadeExtensions.GetPendingMigrationsAsync(dbContext.Database)).ToList();
            if (pendingMigrations.Any())
            {
                Reporter($"  Applying {pendingMigrations.Count} pending migration(s)...");
                await dbContext.Database.MigrateAsync();
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
            string connString = null,
            ILoggerFactory loggerFactory = null)
        {
            var serverType = dbType ?? config.ServerType();
            var connectionString = connString ?? config.ConnectionString();

            return new ChessGamesDbContext(serverType, connectionString, loggerFactory);
        }
    }
}