using System;
using System.Linq;
using AutoMapper;
using chess.games.db.api;
using chess.games.db.api.Services;
using chess.games.db.Entities;
using chess.games.db.pgnimporter.Mapping;
using Microsoft.Extensions.Configuration;

namespace chess.games.db.pgnimporter
{
    class Program
    {
        private static ChessGamesDbContext _dbContext;
        private static readonly PgnFileFinder Finder = new PgnFileFinder();
        private static IConfiguration _config;
        private static IMapper _mapper;
        private static IPgnImportService _svc;
        private static void RaiseStatus(string status) => Console.Write(status);
        static void Main(string[] args)
        {
            Startup();

            RaiseStatus("Performing any DB migrations...\n");
            _dbContext.UpdateDatabase();
            
            var scanPath = args.Any() ? args[0] : @"";

            if (scanPath != "")
            {
                RaiseStatus($"Starting import from: {scanPath}\n");
                var pgnFiles = Finder.FindFiles(scanPath);

                _svc.ImportGames(pgnFiles);
            }
            else
            {
                RaiseStatus("No pgn files/folders specified for input.\n");
            }

            _svc.ProcessUnvalidatedGames();
        }

        private static void Startup()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, false)
                .Build();

            var connectionString = _config["chess-games-db"];

            _dbContext = new ChessGamesDbContext(connectionString);
            _mapper = AutoMapperFactory.Create();
            var pgnRepository = new PgnRepository(_dbContext);
            _svc = new PgnImportService(pgnRepository, _mapper);
            _svc.Status += RaiseStatus;
        }

    }
}
