using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api
{
    public interface IPgnRepository
    {
        long ImportQueueSize { get; }
        int QueuePgnGames(IEnumerable<PgnImport> games);
    }
}