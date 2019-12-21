using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api
{
    public class PgnImportQueueRepository : IPgnImportQueueRepository
    {
        private readonly ChessGamesDbContext _chessGamesDbContext;

        public PgnImportQueueRepository(ChessGamesDbContext chessGamesDbContext)
        {
            _chessGamesDbContext = chessGamesDbContext;
        }

        public long ImportQueueSize => _chessGamesDbContext.PgnImportQueue.Count();

        public int AddGamesToPgnImportQueue(IEnumerable<PgnImportQueue> games)
        {
                _chessGamesDbContext.PgnImportQueue
                    .AttachRange(games);

                // NOTE: Does not account for any duplicate entries, SQL unique constrains only work on PK/FK fields
                // TODO: Implement the SQL MERGE approach
                return _chessGamesDbContext.SaveChanges();
        }
        //        private static string MergeNewGamesSql = 
        //            "INSERT Games (Id, EventId, SiteId, WhiteId, BlackId, Date, [Round], Result, MoveText, ECO) " +
        //            "SELECT Id, EventId, SiteId, WhiteId, BlackId, Date, [Round], Result, MoveText, ECO " +
        //            "FROM GameImports imp " +
        //            "WHERE NOT EXISTS (SELECT " +
        //            "EventId, SiteId, WhiteId, BlackId, Date, [Round], Result, MoveText, ECO " +
        //            "FROM Games g2 WHERE " +
        //            "g2.EventId = imp.EventId " +
        //            "AND g2.SiteId = imp.SiteId " +
        //            "AND g2.WhiteId = imp.WhiteId " +
        //            "AND g2.BlackId = imp.BlackId " +
        //            "AND g2.Date = imp.Date " +
        //            "AND g2.[Round] = imp.[Round] " +
        //            "AND g2.Result = imp.Result " +
        //            "AND g2.MoveText = imp.MoveText " +
        //            "AND g2.ECO = imp.ECO " +
        //            ")";

    }
}
