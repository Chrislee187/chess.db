using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using chess.games.db.api;
using chess.games.db.Entities;
using chess.games.db.pgnimporter.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PgnReader;

namespace chess.games.db.pgnimporter
{
    class Program
    {
        private static ChessGamesDbContext _dbContext;
        private static readonly PgnFileArchiver Archiver = new PgnFileArchiver();
        private static readonly PgnFileFinder Finder = new PgnFileFinder();
        private static IConfiguration _config;
        private static IMapper _mapper;

        static void Main(string[] args)
        {
            Startup();

            var connectionString = _config["chess-games-db"];

            Console.WriteLine("Initialising DB Connection...");
            _dbContext = new ChessGamesDbContext(connectionString);

            Console.WriteLine("Updating DB...");

            _dbContext.UpdateDatabase();
            var scanPath = args.Any() ? args[0] : @".\";

            var pgnFiles = Finder.FindFiles(scanPath);

            ImportGames(pgnFiles);
            
        }

        private static void Startup()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, false)
                .Build();


            _mapper = AutoMapperFactory.Create();
        }

        private static void ImportGames(string[] pgnFiles)
        {
            Console.WriteLine("Initialising repo and cache...");
            
            IPgnImportQueueRepository repo = new PgnImportQueueRepository(_dbContext);
            Console.WriteLine($"Beginning import of {pgnFiles.Length} PGN files at: {DateTime.Now}");

            var fileCount = 0;
            var avgs = new List<int>();
            pgnFiles.ToList().ForEach(file =>
            {
                fileCount++;
                try
                {
                    Console.WriteLine($"File #{fileCount}/{pgnFiles.Length} : {file}");

                    var pgnGames = PgnGame.ReadAllGamesFromFile(file).ToArray();

                    var importGames = pgnGames.Select(_mapper.Map<PgnImportQueue>).ToList();

                    Console.WriteLine($"Adding {importGames.Count()} games for new entries...");
                    
                    var sw = Stopwatch.StartNew();
                    var createdCount = repo.AddGamesToPgnImportQueue(importGames);
                    sw.Stop();

                    Console.WriteLine(
                        $"  File complete, {createdCount} new games added to DB (file contained {pgnGames.Count() - createdCount} duplicates) , DB Total Games: {repo.ImportQueueSize}");
                    var createdPerSec = sw.Elapsed.Seconds == 0 ? createdCount : createdCount / sw.Elapsed.Seconds;
                    avgs.Add(createdPerSec);
                    Console.WriteLine(
                        $"  time taken: {sw.Elapsed}, games created per second: {createdPerSec}");

//                    Archiver.ArchiveImportedFile(file, scanPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: Importing file: {file}");
                    Console.WriteLine(e);

                    //                    var failPath = Archiver.ArchiveFailedFile(file, scanPath);
                    //                    Console.WriteLine($"Fail archived at: {failPath}");

                    if (e is SqlException) throw;
                }

                if (avgs.Any())
                {
                    Console.WriteLine($"  Average games created per second: {avgs.Average()}.");
                }
            });
            Console.WriteLine($"  Final Average games created per second: {avgs.Average()}");
        }


    }
}
