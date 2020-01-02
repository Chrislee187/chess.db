using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;
using chess.games.db.Extensions;
using Microsoft.EntityFrameworkCore;

namespace chess.games.db.api
{
    public class PgnRepository : IPgnRepository
    {
        private readonly ChessGamesDbContext _database;

        public bool ContainsGame(Game game)
        {
            if (!_database.Games.Any(g => g.MoveText == game.MoveText)) return false;

            return _database.Games.Any(g =>
                g.Event == game.Event
                && g.Site == game.Site
                && g.White == game.White
                && g.Black == game.Black
                && g.Result == game.Result
                && g.Round == game.Round
                && g.Date == game.Date
                && g.WhiteElo == game.WhiteElo
                && g.BlackElo == game.BlackElo
                && g.Eco == game.Eco
                );
        }

        public void SaveChanges()
        {
            _database.SaveChanges();
        }

        public PgnRepository(ChessGamesDbContext database)
        {
            _database = database;
        }

        public int ImportQueueSize
            => _database.PgnGames.AsNoTracking()
                .Count(g => !_database.ImportedPgnGameIds.AsNoTracking()
                                .Any(i => i.Id == g.Id)
                            && !_database.PgnImportErrors.AsNoTracking()
                                .Any(e => e.PgnGameId == g.Id));

        public IEnumerable<PgnGame> ValidationBatch(int batchSize = 1000)
            => _database.PgnGames.AsNoTracking()
                .Where(g => !_database.ImportedPgnGameIds.AsNoTracking()
                                .Any(i => i.Id == g.Id)
                            && !_database.PgnImportErrors.AsNoTracking()
                                .Any(e => e.PgnGameId == g.Id))
                .Take(batchSize);

        public int QueuePgnGames(IEnumerable<PgnImport> games)
        {
            var gamesList = games.ToArray();

            _database.PgnImports.AttachRange(gamesList);
			_database.SaveChanges();

            // NOTE: Merge new games in to main PgnGame store, we use raw SQL here as it's exponentially quicker
            // than per rows scans with EF Core DbSets
            int mergedGamesCount = 0;
            _database.RunWithExtendedTimeout(() =>
            {
                mergedGamesCount = _database.Database.ExecuteSqlRaw(MergeNewGamesSql);
            }, TimeSpan.FromMinutes(5));

			// Clean the import queue
            _database.PgnImports.RemoveRange(gamesList);
            _database.SaveChanges();

			return mergedGamesCount;
        }

        public void AddNewGame(Game game) 
            => _database.Games.Add(game);

        public Game CreateGame(PgnGame pgnGame)
        {
            var pgnEvent = FindOrCreateEvent(pgnGame);
            var pgnSite = FindOrCreateSite(pgnGame);
            var black = FindOrCreatePlayer(pgnGame.Black);
            var white = FindOrCreatePlayer(pgnGame.White);

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

                return game;
            }
            int? SafeElo(string value)
            {
                if (string.IsNullOrEmpty(value)) return null;

                if (int.TryParse(value, out int v))
                {
                    return v;
                }

                return null;
            }

            return null;
        }

        public void MarkGameImported(Guid id) 
            => _database.ImportedPgnGameIds.Add(new ImportedPgnGameIds(id));

        public void MarkGameImportFailed(Guid errorGameId, string errorMessage) 
            => _database.PgnImportErrors.Add(new PgnImportError(errorGameId, errorMessage));

        private PgnPlayer MatchPlayer(string pgnPlayerName)
        {
            if (!PersonName.TryParse(pgnPlayerName, out var personName)) return null;

            var pgnPlayer = _database.PlayerLookup.Find(pgnPlayerName);
            if (pgnPlayer != null)
            {
                LoadChildren(pgnPlayer);
                return pgnPlayer;
            }

            var matcher = new PersonNameMatcher();

            var surnameRelations = _database.Players
                .Where(p => p.LastName == personName.Lastname)
                .ToList();

            var match = matcher.Match(personName, surnameRelations);

            if(match != null)
            {
                return CreatePlayerLookup(pgnPlayerName, match);
            }

            return CreatePlayerLookup(pgnPlayerName, personName);
        }

        private PgnPlayer CreatePlayerLookup(string pgnName, Player player)
        {
            var matchPlayer = new PgnPlayer()
            {
                Id = pgnName,
                Player = player
            };

            _database.PlayerLookup.Add(matchPlayer);
            return matchPlayer;
        }

        private PgnPlayer CreatePlayerLookup(string pgnName, PersonName personName)
        {
            var player = new Player()
            {
                Id = Guid.NewGuid(),
                Firstname = personName.Firstname,
                LastName = personName.Lastname,
                OtherNames = personName.Middlename
            };
            _database.Players.Add(player);
            var lookup = new PgnPlayer()
            {
                Id = pgnName,
                Player = player
            };
            _database.PlayerLookup.Add(lookup);
            return lookup;
        }

        private PgnPlayer FindOrCreatePlayer(string pgnName)
        {
            var lookup = _database.PlayerLookup.Find(pgnName);

            if (lookup != null)
            {
                LoadChildren(lookup);

                return lookup;
            }

            return MatchPlayer(pgnName);
        }

        private PgnEvent FindOrCreateEvent(PgnGame pgnGame)
        {
            var lookup = _database.EventLookup.Find(pgnGame.Event);
            if (lookup == null)
            {
                lookup = new PgnEvent
                {
                    Id = pgnGame.Event,
                    Event = new Event {Id = Guid.NewGuid(), Name = pgnGame.Event}
                };
                _database.EventLookup.Add(lookup);
            }
            else
            {
                LoadChildren(lookup);
            }

            return lookup;
        }

        private PgnSite FindOrCreateSite(PgnGame pgnGame)
        {
            var lookup = _database.SiteLookup.Find(pgnGame.Site);
            if (lookup == null)
            {
                lookup = new PgnSite()
                {
                    Id = pgnGame.Site,
                    Site = new Site() { Id = Guid.NewGuid(), Name = pgnGame.Site }
                };

                _database.SiteLookup.Add(lookup);
            }
            else
            {
                LoadChildren(lookup);
            }

            return lookup;
        }

        private void LoadChildren(PgnPlayer entity) 
            => _database.LoadChildren(entity,"Player");

        private void LoadChildren(PgnSite entity)
            => _database.LoadChildren(entity, "Site");

        private void LoadChildren(PgnEvent entity)
            => _database.LoadChildren(entity, "Event");

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
    }
}
