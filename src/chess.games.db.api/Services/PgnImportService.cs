using AutoMapper;
using chess.games.db.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PgnGame = PgnReader.PgnGame;

namespace chess.games.db.api.Services
{
    /// <summary>
    /// Import raw PGN files, has logic to ignore duplicate PGN entries
    /// </summary>
    public class PgnImportService : IPgnImportService
    {
        private readonly IPgnRepository _pgnRepository;
        private readonly IMapper _mapper;
        public event Action<string> Status;

        private void RaiseStatus(string status) => Status?.Invoke(status);

        public PgnImportService(IPgnRepository pgnRepository, IMapper mapper)
        {
            _mapper = mapper;
            _pgnRepository = pgnRepository;

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
                    var createdCount = _pgnRepository.QueuePgnGames(importGames);
                    sw.Stop();

                    RaiseStatus($"{createdCount}".PadLeft(7) + $"{pgnGames.Length - createdCount}".PadLeft(7));

                    var createdPerSec = createdCount * (1000 / sw.Elapsed.Milliseconds);
                    averages.Add(createdPerSec);

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
            RaiseStatus($"\n{pgnFiles.Length} total files processed, {averages.Average()} games created per second");
        }

        public void ProcessUnvalidatedGames()
        {
            var batchSize = 5001;
            var games = _pgnRepository.ValidationBatch(batchSize).ToList();

            if (!games.Any())
            {
                RaiseStatus("\nNo outstanding imported games requiring validation.\n");
            }

            while (games.Any())
            {
                var count = _pgnRepository.PgnGameCount();
                RaiseStatus($"\nLoading {games.Count} unvalidated game(s) (from {count} outstanding) for analysis...\n");

                int? SafeElo(string value)
                {
                    if (string.IsNullOrEmpty(value)) return null;

                    if (int.TryParse(value, out int v))
                    {
                        return v;
                    }

                    return null;
                }

                var gamesWithUnMatchedPlayers = new List<Entities.PgnGame>();
                var gamesAdded = 0;
                var dupesFound = 0;

                var sw = Stopwatch.StartNew();

                foreach (var pgnGame in games)
                {
                    var pgnEvent = _pgnRepository.FindOrCreateEvent(pgnGame);
                    var pgnSite = _pgnRepository.FindOrCreateSite(pgnGame);
                    var black = _pgnRepository.FindOrCreatePlayer(pgnGame.Black);
                    var white = _pgnRepository.FindOrCreatePlayer(pgnGame.White);

                    if (black != null && white != null)
                    {
                        var game = new Game
                        {
                            Id = Guid.NewGuid(),
                            Event = pgnEvent.Event,
                            Site = pgnSite.Site,
                            White = white.Player,
                            Black = black.Player,
                            Date = pgnGame.Date,
                            Round = pgnGame.Round,
                            Result = pgnGame.Result.ToGameResult(),
                            WhiteElo = SafeElo(pgnGame.WhiteElo),
                            BlackElo = SafeElo(pgnGame.BlackElo),
                            Eco = pgnGame.Eco,
                            MoveText = pgnGame.MoveList,
                        };

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
                        _pgnRepository.SaveChanges();
                    }
                    else
                    {
                        gamesWithUnMatchedPlayers.Add(pgnGame);
                    }

                    var progress = gamesAdded + dupesFound;
                    progress = progress == 0 ? 1 : progress;
                    var interval = 100;
                    if (progress % interval == 0)
                    {
                        RaiseStatus("+");
                        if (progress % 1000 == 0)
                        {
                            sw.Stop();
                            RaiseStatus(
                                $"\n 1000 games processed in {sw.Elapsed} - {sw.Elapsed.TotalMilliseconds / 1000:#}ms avg per game\n");
                            sw.Restart();
                        }
                    }
                }
                sw.Stop();

                RaiseStatus("\n");
                RaiseStatus($"{gamesAdded} games added to the DB.\n");
                RaiseStatus($"{dupesFound} duplicates ignored.\n");

                if (gamesWithUnMatchedPlayers.Any())
                {
                    foreach (var errorGame in gamesWithUnMatchedPlayers)
                    {
                        _pgnRepository.MarkGameImportFailed(errorGame.Id, "unmatched-player");
                    }
                }

                _pgnRepository.SaveChanges();
                games = _pgnRepository.ValidationBatch(batchSize).ToList();
            }
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
    }
}