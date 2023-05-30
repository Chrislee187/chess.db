using System.Collections;
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

            _logger.LogInformation("Import game {gameCount} : {White} vs {Black}, {Event} Round {Round}",
                       ++gameCount, pgnGame.White, pgnGame.Black, pgnGame.Event, pgnGame.Round);
            if (_gameIndex.TryAdd(game, out var existing))
            {
                _logger.LogInformation("...validating game");

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

    private void ValidateGame(PgnGame pgnGame, GameEntity gameEntity)
    {
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
            var serialized = GetSerializedBoardState(gameReplay);

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

    private (long boardMask, Guid pieceMask, bool whiteCanCastle, bool blackCanCastle) GetSerializedBoardState(ChessGame gameReplay)
    {
        var serializedBoard = SerializeBoardKey(gameReplay.Board);
        var whiteKing = gameReplay.BoardState
            .GetItems((int)Colours.White, (int)ChessPieceName.King).Single();
        var blackKing = gameReplay.BoardState
            .GetItems((int)Colours.Black, (int)ChessPieceName.King).Single();
        var whiteCanCastle = !whiteKing.Item.LocationHistory.Any();
        var blackCanCastle = !blackKing.Item.LocationHistory.Any();

        return (serializedBoard.boardMask, serializedBoard.pieceMask, whiteCanCastle, blackCanCastle);
    }

    private (long boardMask, Guid pieceMask) SerializeBoardKey(LocatedItem<ChessPieceEntity>[,] gameReplayBoard)
    {
        var boardMaskBits = new BitArray(64);
        var idx = 0;
        var pieces = new List<byte>();

        // Go top to bottom, right to left
        for (int file = 7; file >= 0; file--)
        {
            for (int rank = 0; rank <= 7; rank++)
            {
                var piece = gameReplayBoard[rank, file];
                boardMaskBits.Set(idx++, piece == null ? false : true);

                if (piece != null)
                {
                    var pieceValue = EncodePieceInto4Bits(piece);
                    pieces.Add(pieceValue);
                }
            }
        }

        var boardMask = EncodeBoardMaskIntoLong(boardMaskBits);
        var pieceMask = EncodePiecesIntoGuid(pieces);


        return (boardMask, pieceMask);
    }

    private static Guid EncodePiecesIntoGuid(List<byte> pieces)
    {
        if (pieces.Count != 32)
        {
            byte[] padding = new byte[32 - pieces.Count];
            pieces.AddRange(padding);
        }
        var malformedGuid = pieces.Aggregate("", (s, a) => s + a.ToString("x"));

        var pseudoGuid = malformedGuid[0..8] + "-"
                                             + malformedGuid[8..12] + "-"
                                             + malformedGuid[12..16] + "-"
                                             + malformedGuid[16..20] + "-"
                                             + malformedGuid[20..];
        var pieceMask = Guid.Parse(pseudoGuid);
        return pieceMask;
    }

    private static long EncodeBoardMaskIntoLong(BitArray boardMaskBits)
    {
        var array = new byte[8];
        boardMaskBits.CopyTo(array, 0);
        return BitConverter.ToInt64(array, 0);
    }

    private static byte EncodePieceInto4Bits(LocatedItem<ChessPieceEntity> l)
    {
        var piece = l.Item.Piece;
        byte pieceValue = piece switch
        {
            ChessPieceName.Pawn => 1,
            ChessPieceName.Rook => 2,
            ChessPieceName.Bishop => 3,
            ChessPieceName.Knight => 4,
            ChessPieceName.Queen => 5,
            ChessPieceName.King => 6,
            _ => throw new ArgumentOutOfRangeException()
        };
        if (l.Item.Player == Colours.Black)
        {
            pieceValue += 8;
        }

        return pieceValue;
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