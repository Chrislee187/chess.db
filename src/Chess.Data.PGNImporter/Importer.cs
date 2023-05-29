using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;
using Chess.Games.Data.Services;
using Microsoft.Extensions.Logging;
using PgnReader;

namespace Chess.Data.PGNImporter;

public class Importer : IImporter
{
    private readonly ILogger<Importer> _logger;
    private readonly IEventIndexingService _eventIndex;
    private readonly ISiteIndexingService _siteIndex;
    private readonly IPlayerIndexingService _playerIndex;
    private readonly IGameIndexingService _gameIndex;
    private readonly IEventRepository _eventRepo;

    public Importer(
        ILogger<Importer> logger,
        IEventRepository eventRepo,
        IEventIndexingService eventIndex,
        ISiteIndexingService siteIndex,
        IPlayerIndexingService playerIndex,
        IGameIndexingService gameIndex)
    {
        _eventRepo = eventRepo;
        _eventIndex = eventIndex;
        _siteIndex = siteIndex;
        _playerIndex = playerIndex;
        _gameIndex = gameIndex;
        _logger = logger;
    }

    public IEnumerable<GameEntity> ImportGames(IEnumerable<PgnGame> games)
    {
        var gameCount = 0;
        var added = new List<GameEntity>();
        foreach (var pgnGame in games)
        {
            var evt = _eventIndex.TryAdd(pgnGame.Event);
            var site = _siteIndex.TryAdd(pgnGame.Site);
            var black = _playerIndex.TryAdd(pgnGame.Black);
            var white = _playerIndex.TryAdd(pgnGame.White);
            int.TryParse(pgnGame.Round, out int round);
            var matchDate = pgnGame.Date.ToDateTime();

            var game = new GameEntity
            {
                SourceEventText = pgnGame.Event,
                Event = evt,
                SourceSiteText = pgnGame.Site,
                Site = site,
                SourceWhitePlayerText = pgnGame.White,
                White = white,
                SourceBlackPlayerText = pgnGame.Black,
                Black = black,
                Date = matchDate,
                Round = round,
                Result = PgnGameResultToGameResult(pgnGame.Result),
                SourceMoveText = pgnGame.MoveText
            };

            _logger.LogInformation("{gameCount} : Checking for game: {White} vs {Black}, {Event} Round {Round}", ++gameCount, pgnGame.White, pgnGame.Black, pgnGame.Event, pgnGame.Round);
            if (_gameIndex.TryAdd(game))
            {
                _logger.LogInformation("...adding game to DB");
                added.Add(game);
                _eventRepo.Save(); // NOTE: This is the "unit-of-work" commit call (DbContext.SaveChanges())
            }
            else
            {
                _logger.LogInformation("...game already exists in DB");
            }
        }

        return added;
    }

    private GameResult PgnGameResultToGameResult(PgnGameResult pgnGameResult) =>
        pgnGameResult switch
        {
            PgnGameResult.Draw => GameResult.Draw,
            PgnGameResult.WhiteWins => GameResult.WhiteWins,
            PgnGameResult.BlackWins => GameResult.BlackWins,
            _ => GameResult.Unknown
        };
}

public interface IImporter
{
    IEnumerable<GameEntity> ImportGames(IEnumerable<PgnGame> games);
}