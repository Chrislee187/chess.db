using chess.games.db.Entities;

namespace chess.games.db.api.Repositories
{
	/// <summary>
	/// Performing duplication checks on a large SQLServer table (2m+ records) on each new insert
	/// was way to (`O(N)`) slow. So a inline SQL approach was taken, which, whilst the performance
	/// does get noticeably slower as the data-set grows it was still faster than doing the same checks
	/// through EF.
	///
	/// SQLite (support added purely for the convenience of having a inproc DB) doesn't support 
	/// the full set of T-SQL that SQLServer does and as such we do not perform any checks
	/// at this stage and simply copy the games to pre-validation table.
	///
	/// A later part of the import process will do further duplicate checks based on the names and will
	/// catch any dupes that this process missed.
	/// </summary> 
	public class MergeSqlFactory
    {
        public string Create(in bool isSqlLite)
            => isSqlLite ? MergeNewGamesSqlite : MergeNewGamesSql;

        private static readonly string MergeNewGamesSql = $@"INSERT {nameof(ChessGamesDbContext.PgnGames)} (
		Id, [Event], [Site], White, Black, [Date], [Round], 
		Result, MoveList, ECO, WhiteElo, BlackElo, CustomTagsJson
	) 
SELECT newid(), [Event], [Site], White, Black, [Date], [Round], 
		Result, MoveList, ECO, WhiteElo, BlackElo, CustomTagsJson
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

        private static string MergeNewGamesSqlite =
            $@"INSERT INTO {nameof(ChessGamesDbContext.PgnGames)} (
		Id, [Event], [Site], White, Black, [Date], [Round], 
		Result, MoveList, ECO, WhiteElo, BlackElo, CustomTagsJson
	) 
SELECT Id, [Event], [Site], White, Black, [Date], [Round], 
		Result, MoveList, ECO, WhiteElo, BlackElo, CustomTagsJson
	FROM 	{nameof(ChessGamesDbContext.PgnImports)} import 

";
    }
}