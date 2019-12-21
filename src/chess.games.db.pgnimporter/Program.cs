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

        static void Main(string[] args)
        {
            Startup();

            var connectionString = _config["chess-games-db"];

            Console.WriteLine("Initialising DB Connection...");
            _dbContext = new ChessGamesDbContext(connectionString);

            Console.WriteLine("Performing any DB migrations...");
            _dbContext.UpdateDatabase();
            
            var scanPath = args.Any() ? args[0] : @".\";

            var pgnFiles = Finder.FindFiles(scanPath);

            Console.WriteLine("Starting import...");
            ImportGames(pgnFiles);
            
        }

        private static void Startup()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, false)
                .Build();
            
            _mapper = AutoMapperFactory.Create();
        }

        private static void Status(string msg) => Console.WriteLine(msg);

        private static void ImportGames(string[] pgnFiles)
        {
            // TODO: Setup IOC with a global static reference to the container
            var svc = new PgnImportService(new PgnRepository(_dbContext), _mapper);

            svc.Status += Status;

            svc.ImportGames(pgnFiles);
            
        }
    }
}
