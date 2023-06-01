using board.engine.Board;
using board.engine;
using chess.engine.Entities;
using chess.engine.Game;
using chess.engine;
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

    private readonly IChessBoardStateSerializer _boardSerializer;
    public Importer(
        ILogger<Importer> logger,
        IEventRepository eventRepo,
        IEventIndexingService eventIndex,
        ISiteIndexingService siteIndex,
        IPlayerIndexingService playerIndex,
        IGameIndexingService gameIndex,
        IChessBoardStateSerializer boardSerializer)
    {
        _eventRepo = eventRepo;
        _eventIndex = eventIndex;
        _siteIndex = siteIndex;
        _playerIndex = playerIndex;
        _gameIndex = gameIndex;
        _logger = logger;
        _boardSerializer = boardSerializer;
    }

    public IEnumerable<GameEntity> ImportGames(IEnumerable<PgnGame> games)
    {
        var gameCount = 0;
        var added = new List<GameEntity>();
        foreach (var pgnGame in games)
        {
            _logger.LogInformation("Import game {gameCount} : {White} vs {Black}, {Event} Round {Round}",
                ++gameCount, pgnGame.White, pgnGame.Black, pgnGame.Event, pgnGame.Round);

            var game = CreatePotentialGameEntity(pgnGame);

            if (_gameIndex.TryAdd(game, out var existing))
            {
                ValidateGame(pgnGame, game);
                
                _logger.LogInformation("...importing game");
                added.Add(game);
                _eventRepo.Save(); // NOTE: This is the "unit-of-work" commit call to DbContext.SaveChanges()
            }
            else
            {
                _logger.LogInformation("...game already imported. {GameId}", existing!.Id);
            }


        }

        return added;
    }

    private GameEntity CreatePotentialGameEntity(PgnGame pgnGame)
    {
        var evt = _eventIndex.TryAdd(pgnGame.Event);
        var site = _siteIndex.TryAdd(pgnGame.Site);
        var black = _playerIndex.TryAdd(pgnGame.Black);
        var white = _playerIndex.TryAdd(pgnGame.White);
        int.TryParse(pgnGame.Round, out var round);
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
        return game;
    }

    private void ValidateGame(PgnGame pgnGame, GameEntity gameEntity)
    {
        _logger.LogInformation("...validating game");

        var checkDetectionService = AppContainer.GetService<ICheckDetectionService>();
        var engineProvider = AppContainer.GetService<IBoardEngineProvider<ChessPieceEntity>>();
        var entityFactory = AppContainer.GetService<IBoardEntityFactory<ChessPieceEntity>>();
        var gameReplay = new ChessGame(engineProvider, entityFactory, checkDetectionService);

        var lanMoveList = new List<string>();
        foreach (var turn in pgnGame.Turns)
        {
            var result = gameReplay.Move(turn.White.San);
            lanMoveList.Add(result.Lan);

            // TODO: this can be used as a UNIQUE key for this exact board position and state
            // var serialized = _boardSerializer.GetSerializedBoardState(gameReplay.Board, gameReplay.BoardState);

            if (turn.Black != null)
            {
                result = gameReplay.Move(turn.Black.San);
                lanMoveList.Add(result.Lan);
            }
        }

        gameEntity.LanMoveText = lanMoveList.Aggregate("", (s, a) => s + a);
        gameEntity.LanMoveTextHash = gameEntity.LanMoveText.GetHashCode();

        // TODO: Check that the movetext doesn't already exist in a game somewhere to try and catch and dupe misses on the events/player names etc.
        // TODO: Check pgnGame final check state matches gameReplay
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