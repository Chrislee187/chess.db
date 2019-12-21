using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using chess.games.db.Entities;
using Microsoft.Data.SqlClient;
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
            RaiseStatus("Initialising repo and cache...");
            RaiseStatus($"Beginning import of {pgnFiles.Length} PGN files at: {DateTime.Now}");

            var fileCount = 0;
            var averages = new List<int>();
            pgnFiles.ToList().ForEach(file =>
            {
                fileCount++;
                try
                {
                    RaiseStatus($"File #{fileCount}/{pgnFiles.Length}\n  {file}");

                    var pgnGames = PgnGame.ReadAllGamesFromFile(file).ToArray();

                    var importGames = pgnGames
                        .Select(_mapper.Map<PgnImport>)
                        .Distinct(PgnImport.PgnDeDupeQueueComparer) // NOTE: Ensures no duplicates games within the file
                        .ToList();

                    RaiseStatus($"  {importGames.Count} unique games found for importing...");

                    var sw = Stopwatch.StartNew();
                    var createdCount = _pgnRepository.QueuePgnGames(importGames);
                    sw.Stop();

                    RaiseStatus($"  File complete, {createdCount} new games added (file contained {pgnGames.Length - createdCount} duplicates)");
                    
                    var createdPerSec = sw.Elapsed.Seconds == 0 ? createdCount : createdCount / sw.Elapsed.Seconds;
                    averages.Add(createdPerSec);
                    
                    RaiseStatus($"  time taken: {sw.Elapsed}, games created per second: {createdPerSec}");
                }
                catch (Exception e)
                {
                    RaiseStatus($"ERROR: Importing file: {file}");
                    RaiseStatus(e.ToString());

                    if (e is SqlException) throw;
                }

                if (averages.Any())
                {
                    RaiseStatus($"  file processed: {averages.Average()} games created per second.");
                }
            });
            RaiseStatus($"{pgnFiles.Length} total files processed, {averages.Average()} games created per second");
        }

    }
}