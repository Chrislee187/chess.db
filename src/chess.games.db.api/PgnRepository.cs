using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;

namespace chess.games.db.api
{
    public class PgnRepository : IPgnRepository
    {
        private readonly ChessGamesDbContext _chessGamesDbContext;

        public PgnRepository(ChessGamesDbContext chessGamesDbContext)
        {
            _chessGamesDbContext = chessGamesDbContext;
        }

        public long ImportQueueSize => _chessGamesDbContext.PgnGames.Count();

        public int QueuePgnGames(IEnumerable<PgnImport> games)
        {
            var gamesList = games.ToArray();

            _chessGamesDbContext.PgnImports.AttachRange(gamesList);
			_chessGamesDbContext.SaveChanges();

			// NOTE: Merge new games in to main PgnGame store, we use raw SQL here as it's exponentially quicker
			// than per rows scans with EF Core DbSets
            var mergedGamesCount = _chessGamesDbContext.Database.ExecuteSqlRaw(MergeNewGamesSql);

			// Clean the import queue
            _chessGamesDbContext.PgnImports.RemoveRange(gamesList);
            _chessGamesDbContext.SaveChanges();

			return mergedGamesCount;
        }
        private static readonly string MergeNewGamesSql = $@"INSERT {nameof(ChessGamesDbContext.PgnGames)} (
		Id, [Event], [Site], White, Black, [Date], [Round], 
		Result, MoveList, ECO, WhiteElo, BlackElo, CustomTagsJson, ImportNormalisationComplete
	) 
SELECT newid(), [Event], [Site], White, Black, [Date], [Round], 
		Result, MoveList, ECO, WhiteElo, BlackElo, CustomTagsJson, 0
	FROM 	{nameof(ChessGamesDbContext.PgnImports)} import 
	WHERE NOT EXISTS (
			SELECT Id
			FROM {nameof(ChessGamesDbContext.PgnGames)} game 
			WHERE
				game.[Event] = import.[Event]
				AND game.[Site] = import.[Site]
				AND game.White = import.White 
				AND game.Black = import.Black 
				AND game.[Date] = import.[Date] 
				AND game.[Round] = import.[Round] 
				AND game.Result = import.Result 
				AND game.MoveList = import.MoveList 
				AND (
						(game.ECO IS NOT NULL AND game.ECO = import.ECO)
						OR (game.ECO IS NULL AND import.Eco IS NULL)
					)
				AND (
						(game.WhiteElo IS NOT NULL AND game.WhiteElo = import.WhiteElo)
						OR (game.WhiteElo IS NULL AND import.WhiteElo IS NULL)
					)
				AND (
						(game.BlackElo IS NOT NULL AND game.BlackElo = import.BlackElo)
						OR (game.BlackElo IS NULL AND import.BlackElo IS NULL)
					)
				AND (
						(game.CustomTagsJson IS NOT NULL AND game.CustomTagsJson = import.CustomTagsJson)
						OR (game.CustomTagsJson IS NULL AND import.CustomTagsJson IS NULL)
					)
		);
";

    }
}
