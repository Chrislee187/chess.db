using System;
using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api
{
    public interface IPgnRepository
    {
        int ImportQueueSize { get; }
        int QueuePgnGames(IEnumerable<PgnImport> games);
        IEnumerable<PgnGame> ValidationBatch(int batchSize = int.MaxValue);
        void AddNewGame(Game game);

        bool ContainsGame(Game game);
        void SaveChanges();
        void MarkGameImported(Guid id);
        void MarkGameImportFailed(Guid errorGameId, string errorMessage);
        Game CreateGame(PgnGame pgnGame);
    }
}