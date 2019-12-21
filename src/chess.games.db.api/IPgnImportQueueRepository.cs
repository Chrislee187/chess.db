using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api
{
    public interface IPgnImportQueueRepository
    {
        long ImportQueueSize { get; }
        int AddGamesToPgnImportQueue(IEnumerable<PgnImportQueue> games);
    }
}