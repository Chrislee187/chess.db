using System;
using System.IO;
using System.Linq;
using AutoMapper;
using chess.games.db.api.Repositories;
using chess.games.db.api.Services;
using chess.games.db.Entities;
using chess.games.db.pgnimporter.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using ConfigurationExtensions = chess.games.db.Configuration.ConfigurationExtensions;

namespace chess.games.db.pgnimporter
{
    class Program
    {
        public static IConfiguration Configuration { get; private set; }

        private static ChessGamesDbContext _dbContext;
        private static readonly PgnFileFinder Finder = new PgnFileFinder();

        private static IMapper _mapper;
        private static IPgnImportService _svc;
        private static void ShowStatus(string status) => Console.Write(status);
        static void Main(string[] args)
        {
            Startup(args);

            ShowStatus("Performing any DB migrations...\n");
            _dbContext.UpdateDatabase();
            
            var scanPath = args.Any() ? args[0] : @"";

            if (scanPath != "")
            {
                ShowStatus($"Starting import from: {scanPath}...\n");
                Log.Information("{scanPath}", scanPath);

                _svc.ImportGames(Finder.FindFiles(scanPath));
            }
            else
            {
                ShowStatus("No pgn files/folders specified for input.\n");
            }

            ShowStatus("Initialising validation process...\n");

            Console.CursorVisible = false;

            _svc.ProcessUnvalidatedGames();
            Console.CursorVisible = true;
        }

        private static void Startup(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appSettings.json", optional: false)
                .AddCommandLine(args)
                .Build();

            var serverType = Enum.Parse<ConfigurationExtensions.DbServerTypes>(Configuration["DbServerType"]);
            var connectionString = Configuration["ChessDB"];

            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(Configuration)
                .CreateLogger();

            var loggerFactory = new SerilogLoggerFactory(Log.Logger);

            _dbContext = new ChessGamesDbContext(serverType, connectionString, loggerFactory);
            _mapper = AutoMapperFactory.Create();

            var pgnRepository = new PgnRepository(_dbContext, loggerFactory.CreateLogger<PgnRepository>());
            _svc = new PgnImportService(pgnRepository, _mapper, loggerFactory.CreateLogger<PgnImportService>());
            
            _svc.Status += ShowStatus;
        }

    }
}
