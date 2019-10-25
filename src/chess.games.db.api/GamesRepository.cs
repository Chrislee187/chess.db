using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using PgnReader;

namespace chess.games.db.api
{
    public class GamesRepository : IGamesRepository
    {
        private readonly ChessGamesDbContext _chessGamesDbContext;
        private IDictionary<Type, IDictionary<string, object>> _childCache;

        public GamesRepository(ChessGamesDbContext chessGamesDbContext)
        {
            _chessGamesDbContext = chessGamesDbContext;
            CacheChildren();
        }

        private void CacheChildren()
        {
            _chessGamesDbContext.RunWithExtendedTimeout(() =>
            {
                _childCache = new Dictionary<Type, IDictionary<string, object>>
                {
                    {
                        typeof(Event), _chessGamesDbContext.Events.ToList()
                            .GroupBy(k => NormaliseName(k.Name), (k, g) => g.First())
                            .ToDictionary(k => NormaliseName(k.Name), v => (object) v)
                    },
                    {
                        typeof(Site), _chessGamesDbContext.Sites.ToList()
                            .GroupBy(k => NormaliseName(k.Name), (k, g) => g.First())
                            .ToDictionary(k => NormaliseName(k.Name), v => (object) v)
                    },
                    {
                        typeof(PgnPlayer), _chessGamesDbContext.PgnPlayers.ToList()
                            .GroupBy(k => NormaliseName(k.Name), (k, g) => g.First())
                            .ToDictionary(k => NormaliseName(k.Name), v => (object) v)
                    },
                };

            }, TimeSpan.FromMinutes(5));
        }

        public long TotalGames => _chessGamesDbContext.Games.Count();

        public int AddImportBatch(PgnGame[] games)
        {
            throw new NotImplementedException("Not implemented for new table schemas");
            int created = 0;
            _chessGamesDbContext.RunWithExtendedTimeout(() =>
            {
                _chessGamesDbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [GameImports]");

                games.ToList().ForEach(pgnGame =>
                {
                    var site = GetOrCreateCachedEntity<Site>(pgnGame.Site);
                    var whitePlayer = GetOrCreateCachedEntity<PgnPlayer>(pgnGame.White);
                    var blackPlayer = GetOrCreateCachedEntity<PgnPlayer>(pgnGame.Black);
                    var @event = GetOrCreateCachedEntity<Event>(pgnGame.Event);

                    var game = new GameImport
                    {
                        Event = @event,
                        Black = blackPlayer,
                        Site = site,
                        White = whitePlayer,
                        Round = pgnGame.Round,
                        MoveText = NormaliseMoveText(pgnGame),
                        Date = pgnGame.Date.ToString(),
                        Result = ConvertPGNResult(pgnGame.Result)
                    };
                    var pairs = pgnGame.TagPairs.ToDictionary(k => k.Name, v=> v.Value);
                    if (pairs.ContainsKey("ECO"))
                    {
                        game.ECO = pairs["ECO"];
                    }
                    _chessGamesDbContext.GameImports.Attach(game);
                });
                _chessGamesDbContext.SaveChanges();

                var sql = MergeNewGamesSql;
                created = _chessGamesDbContext.Database.ExecuteSqlCommand(sql);
            }, TimeSpan.FromMinutes(15));
            return created;
        }

        public GameResult ConvertPGNResult(PgnGameResult pgnR)
        {
            switch (pgnR)
            {
                case PgnGameResult.Unknown:
                    return GameResult.Unknown;
                case PgnGameResult.Draw:
                    return GameResult.Draw;
                case PgnGameResult.WhiteWins:
                    return GameResult.WhiteWins;
                case PgnGameResult.BlackWins:
                    return GameResult.BlackWins;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pgnR), pgnR, null);
            }
        }
        private static string  MergeNewGamesSql = 
            "INSERT Games (Id, EventId, SiteId, WhiteId, BlackId, Date, [Round], Result, MoveText, ECO) " +
            "SELECT Id, EventId, SiteId, WhiteId, BlackId, Date, [Round], Result, MoveText, ECO " +
            "FROM GameImports imp " +
            "WHERE NOT EXISTS (SELECT " +
            "EventId, SiteId, WhiteId, BlackId, Date, [Round], Result, MoveText, ECO " +
            "FROM Games g2 WHERE " +
            "g2.EventId = imp.EventId " +
            "AND g2.SiteId = imp.SiteId " +
            "AND g2.WhiteId = imp.WhiteId " +
            "AND g2.BlackId = imp.BlackId " +
            "AND g2.Date = imp.Date " +
            "AND g2.[Round] = imp.[Round] " +
            "AND g2.Result = imp.Result " +
            "AND g2.MoveText = imp.MoveText " +
            "AND g2.ECO = imp.ECO " +
            ")";

        private T GetOrCreateCachedEntity<T>(string name) where T : class, IHaveAName
        {
            var cache = _childCache[typeof(T)];
            var normaliseName = NormaliseName(name);
            if (!cache.TryGetValue(normaliseName, out var entity))
            {
                var instance = Activator.CreateInstance<T>();
                instance.Name = name;
                cache.Add(normaliseName, instance);
                entity = instance;
            }

            return (T)entity;

        }
        private static string NormaliseMoveText(PgnGame pgnGame)
        {
            return pgnGame.MoveText
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("  ", " ")
                .Replace("{ ", "{")
                .Replace(" }", "}");
        }
        private static string NormaliseName(string name) =>
            name.ToLower()
                .Replace("-", " ")
                .Replace(".", " ");

    }
}
