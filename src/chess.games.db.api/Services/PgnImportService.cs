using AutoMapper;
using chess.games.db.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using chess.games.db.api.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PgnGame = PgnReader.PgnGame;

namespace chess.games.db.api.Services
{
    public interface IPgnImportService
    {
        event Action<string> Status;
        void ImportGames(string[] pgnFiles);
        void ProcessUnvalidatedGames();
    }
    
    /// <summary>
    /// Import raw PGN files, has logic to ignore duplicate PGN entries
    /// </summary>
    public class PgnImportService : IPgnImportService
    {
        private readonly IPgnRepository _pgnRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PgnImportService> _logger;

        public PgnImportService(
            IPgnRepository pgnRepository, 
            IMapper mapper, 
            ILogger<PgnImportService> logger = null)
        {
            _logger = logger ?? NullLogger<PgnImportService>.Instance;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _pgnRepository = pgnRepository ?? throw new ArgumentNullException(nameof(pgnRepository));
        }

        public void ImportGames(string[] pgnFiles)
        {
            RaiseStatus($"Beginning import of {pgnFiles.Length} PGN files at: {DateTime.Now}\n");

            var fileCount = 0;
            var averages = new List<int>();
            RaiseStatusShowingFileImportHeader();

            pgnFiles.ToList().ForEach(file =>
            {
                fileCount++;
                try
                {
                    RaiseStatus($"({fileCount:G3}/{pgnFiles.Length:G3}) {Path.GetFileName(file)}".PadRight(50));

                    var pgnGames = PgnGame.ReadAllGamesFromFile(file).ToArray();

                    var importGames = pgnGames
                        .Select(_mapper.Map<PgnImport>)
                        .Distinct(PgnImport.PgnDeDupeQueueComparer) // NOTE: Ensures no duplicates games within the file
                        .ToList();

                    RaiseStatus($"{importGames.Count}".PadLeft(7));

                    var sw = Stopwatch.StartNew();
                    var createdCount = _pgnRepository.QueuePgnGamesForValidation(importGames);
                    sw.Stop();

                    RaiseStatus($"{createdCount}".PadLeft(7) + $"{pgnGames.Length - createdCount}".PadLeft(7));

                    var createdPerSec = 0;
                    if (sw.ElapsedMilliseconds > 0)
                    {
                        createdPerSec = createdCount * (1000 / sw.Elapsed.Milliseconds);
                        averages.Add(createdPerSec);
                    }

                    RaiseStatus($"{sw.Elapsed.Milliseconds}ms".PadLeft(7));
                    RaiseStatus($"{createdPerSec}".PadLeft(7));
                    RaiseStatus("\n");
                }
                catch (Exception e)
                {
                    RaiseStatus($"ERROR: {e.Message}\n");

                    if (e is SqlException) throw;
                }
            });
            if (averages.Any())
            {
                RaiseStatus($"\n{pgnFiles.Length} total files processed, {averages.Average()} games created per second.\n");
            }
        }

        public void ProcessUnvalidatedGames()
        {
            var validationBatch = _pgnRepository.ValidationBatch().ToList();
            var wp = validationBatch
                    .GroupBy(s => s.White);

            var bp = validationBatch
                    .GroupBy(s => s.Black);

            var p = wp.Concat(bp)
                .OrderByDescending(g => g.Count());

            var players = p.Select(s => s.Key)
                .Distinct().ToList();

            if (!players.Any())
            {
                RaiseStatus("\nNo outstanding imported games requiring validation.\n");
            }

            RaiseStatus($"\n{players.Count} players left to analyse.\n{_pgnRepository.ImportQueueSize} total games left to analyse...\n");

            var count = 0;
            
            foreach (var player in players)
            {
                var games = _pgnRepository.ValidationBatch(player).ToList();
                _logger.LogDebug("{games} read for '{player}'", games.Count(), player);
                RaiseStatus($"\nValidating {games.Count().ToString().PadLeft(5)} game(s) for ({++count}/{players.Count}) '{player}' ");

                var gamesAdded = 0;
                var dupesFound = 0;
                var failures = 0;

                var sw = Stopwatch.StartNew();

                var ind = new StatusIndicatorBuilder();
                foreach (var pgnGame in games)
                {
                    RaiseStatus($"{ind.Next()}");
                    var game = _pgnRepository.CreateGame(pgnGame);

                    if (game != null)
                    {
                        if (!_pgnRepository.ContainsGame(game))
                        {
                            _pgnRepository.AddNewGame(game);
                            gamesAdded++;
                        }
                        else
                        {
                            dupesFound++;
                        }

                        _pgnRepository.MarkGameImported(pgnGame.Id);
                    }
                    else
                    {
                        failures++;
                        _pgnRepository.MarkGameImportFailed(pgnGame.Id, "unmatched-player");
                    }

                    _pgnRepository.SaveChanges();
                }
                sw.Stop();
                RaiseStatus(ind.ClearLast());
                RaiseStatusSummary(gamesAdded, dupesFound, failures, sw);
            }
        }
        
        private void RaiseStatusSummary(int gamesAdded, int dupesFound, int failures, Stopwatch sw)
        {
            var totalProcessed = gamesAdded + dupesFound + failures;
            
            var avgMs = totalProcessed == 0 ? 0 : sw.ElapsedMilliseconds / totalProcessed;
            
            RaiseStatus($"\t+{gamesAdded}={dupesFound}?{failures} - {avgMs}ms avg. per game");
        }

        private void RaiseStatusShowingFileImportHeader()
        {
            RaiseStatus($"File".PadRight(50));
            RaiseStatus($"#".PadLeft(7));
            RaiseStatus($"+".PadLeft(7));
            RaiseStatus($"dupe".PadLeft(7));
            RaiseStatus($"elpsed".PadLeft(7));
            RaiseStatus($"p/sec".PadLeft(7));
            RaiseStatus("\n");
        }

        public event Action<string> Status;
        private void RaiseStatus(string status) => Status?.Invoke(status);
    }
}