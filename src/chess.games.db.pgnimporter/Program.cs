using System;
using System.IO;
using System.Linq;
using AutoMapper;
using chess.games.db.api.Repositories;
using chess.games.db.api.Services;
using chess.games.db.Configuration;
using chess.games.db.Entities;
using chess.games.db.pgnimporter.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using ConfigurationExtensions = chess.games.db.Configuration.ConfigurationExtensions;

namespace chess.games.db.pgnimporter
{
    class Program
    {
        private static Action<string> Reporter = Console.WriteLine;
        public static IConfiguration Configuration { get; private set; }

        private static ChessGamesDbContext _dbContext;
        private static readonly PgnFileFinder Finder = new PgnFileFinder();

        private static IMapper _mapper;
        private static IPgnImportService _svc;
        private static void ShowStatus(string status) => Console.Write(status);
        static void Main(string[] args)
        {
            Startup(args);
            
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
            ConfigurationExtensions.Reporter = Reporter;
            
            Configuration = ConfigurationExtensions.Configuration(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(Configuration)
                .CreateLogger();

            var loggerFactory = new SerilogLoggerFactory(Log.Logger);

            _dbContext = ConfigurationExtensions.InitDb(args, loggerFactory).Result;

            _mapper = AutoMapperFactory.Create();

            var pgnRepository = new PgnRepository(_dbContext, loggerFactory.CreateLogger<PgnRepository>());
            _svc = new PgnImportService(pgnRepository, _mapper, loggerFactory.CreateLogger<PgnImportService>());
            
            _svc.Status += ShowStatus;
        }
    }
}
