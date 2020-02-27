using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.api.Services;
using chess.games.db.Entities;
using chess.games.db.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace chess.games.db.api.Repositories
{
    public interface IPgnRepository
    {
        int ImportQueueSize { get; }
        int QueuePgnGames(in IEnumerable<PgnImport> games);
        IEnumerable<PgnGame> ValidationBatch(string name = null, int batchSize = int.MaxValue);
        void AddNewGame(Game game);

        bool ContainsGame(Game game);
        void SaveChanges();
        void MarkGameImported(Guid id);
        void MarkGameImportFailed(Guid errorGameId, string errorMessage);
        Game CreateGame(PgnGame pgnGame);
    }
    
    public class PgnRepository : IPgnRepository
    {
        private MergeSqlFactory _sqlFactory = new MergeSqlFactory();
        private readonly ChessGamesDbContext _database;

        private readonly ILogger<PgnRepository> _logger;

        public PgnRepository(
            ChessGamesDbContext database,
            ILogger<PgnRepository> logger = null)
        {
            _logger = logger ?? NullLogger<PgnRepository>.Instance;
            _database = database;
        }

        public bool ContainsGame(Game game)
        {
            if (!_database.Games.Any(g => g.MoveText == game.MoveText)) return false;
            bool result = false;
            _database.RunWithExtendedTimeout(() => result = _database.Games.Any(g =>
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
            ), TimeSpan.FromMinutes(5));
            return result;
        }

        public void SaveChanges()
        {
            _database.SaveChanges();
        }

        public int ImportQueueSize
            => _database.PgnGames.AsNoTracking()
                .Count(g => !_database.ImportedPgnGameIds.AsNoTracking()
                                .Any(i => i.Id == g.Id)
                            && !_database.PgnImportErrors.AsNoTracking()
                                .Any(e => e.PgnGameId == g.Id));


        public IEnumerable<PgnGame> ValidationBatch(string name = null, int batchSize = int.MaxValue)
        {
            IQueryable<PgnGame> filter;

            if (name != null)
            {
                filter = _database.PgnGames.AsNoTracking()
                    .Where(g => !_database.ImportedPgnGameIds.AsNoTracking()
                                    .Any(i => i.Id == g.Id)
                                && !_database.PgnImportErrors.AsNoTracking()
                                    .Any(e => e.PgnGameId == g.Id)
                                && (g.White == name
                                    || g.Black == name));

            }
            else
            {
                filter = _database.PgnGames.AsNoTracking()
                    .Where(g => !_database.ImportedPgnGameIds.AsNoTracking()
                                    .Any(i => i.Id == g.Id)
                                && !_database.PgnImportErrors.AsNoTracking()
                                    .Any(e => e.PgnGameId == g.Id));
            }



            return filter
                .Take(batchSize);
        }

        public int QueuePgnGames(in IEnumerable<PgnImport> games)
        {
            var gamesList = games.ToArray();

            _database.PgnImports.AttachRange(gamesList);
			_database.SaveChanges();

            var isSqlLite =
                _database.Database.ProviderName.Contains("SQLite", StringComparison.InvariantCultureIgnoreCase);

            // NOTE: Merge new games in to main PgnGame store, we use raw SQL here as it's exponentially quicker
            // than per rows scans with EF Core DbSets
            int mergedGamesCount = 0;
            _database.RunWithExtendedTimeout(() =>
            {
                mergedGamesCount = _database.Database.ExecuteSqlRaw(_sqlFactory.Create(isSqlLite));
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
            var pgnPlayer = _database.PlayerLookup.Find(pgnPlayerName);
            if (pgnPlayer != null)
            {
                LoadChildren(pgnPlayer);
                return pgnPlayer;
            }

            if (!PersonName.TryParse(pgnPlayerName, out var personName)) return null;

            var match = MatchPlayer(personName);

            if(match != null)
            {
                return CreatePlayerLookup(pgnPlayerName, match);
            }

            return CreatePlayerLookup(pgnPlayerName, personName);
        }

        private Player MatchPlayer(PersonName personName)
        {
            var players = _database.Players
                // ReSharper disable once SpecifyStringComparison - need this format of insensitive comparison to
                // allow it to be converted to SQL by EF
                .Where(p => personName.Lastname == p.LastName)
                .ToList();

            if (!players.Any()) return null;

            var exact = players.SingleOrDefault(p => MatchAllNames(personName, p));
            if (exact != null)
            {
                return exact;
            }

            if (string.IsNullOrEmpty(personName.Firstname))
            {
                return null;
            }

            var matchingFirstInitial = players
                .Where(p => !string.IsNullOrEmpty(p.Firstname))
                .Where(p => p.Firstname.ToLowerInvariant()[0] == personName.Firstname.ToLowerInvariant()[0])
                .ToList();

            if (matchingFirstInitial.Any())
            {
                if (matchingFirstInitial.Count() == 1)
                {
                    var player = matchingFirstInitial.First();
                    if (personName.Firstname.Length > player.Firstname.Length)
                    {
                        player.Firstname = personName.Firstname;
                    }
                    return player;
                }
                else
                {
                    // TODO: Use middlename to further disambiguate
                    return null;
                }
            }

            return null;
        }

        private static bool MatchAllNames(PersonName personName, Player player)
        {
            return string.Equals(personName.Firstname, player.Firstname, StringComparison.InvariantCultureIgnoreCase)
                   && string.Equals(personName.Middlename, player.OtherNames, StringComparison.InvariantCultureIgnoreCase)
                   && string.Equals(personName.Lastname, player.LastName, StringComparison.InvariantCultureIgnoreCase);
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
    }
}
