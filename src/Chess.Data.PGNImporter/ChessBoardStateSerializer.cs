using System.Collections;
using board.engine.Board;
using chess.engine.Entities;
using chess.engine.Game;

namespace Chess.Data.PGNImporter;

public interface IChessBoardStateSerializer
{
    (long boardMask, Guid pieceMask, bool whiteCanCastle, bool blackCanCastle) 
        GetSerializedBoardState(LocatedItem<ChessPieceEntity>[,] board, IBoardState<ChessPieceEntity> boardState);
}

public class ChessBoardStateSerializer : IChessBoardStateSerializer
{
    private readonly ChessPiece4BitEncoder _pieceEncoder = new();
    
    public (long boardMask, Guid pieceMask, bool whiteCanCastle, bool blackCanCastle) 
        GetSerializedBoardState(LocatedItem<ChessPieceEntity>[,] board, IBoardState<ChessPieceEntity> boardState)
    {
        var serializedBoard = EncodeBoard(board);
        var whiteKing = boardState
            .GetItems((int)Colours.White, (int)ChessPieceName.King).Single();
        var blackKing = boardState
            .GetItems((int)Colours.Black, (int)ChessPieceName.King).Single();
        var whiteCanCastle = !whiteKing.Item.LocationHistory.Any();
        var blackCanCastle = !blackKing.Item.LocationHistory.Any();

        return (serializedBoard.boardMask, serializedBoard.pieceMask, whiteCanCastle, blackCanCastle);
    }

    private (long boardMask, Guid pieceMask) EncodeBoard(LocatedItem<ChessPieceEntity>[,] gameReplayBoard)
    {
        var boardMask = EncodeBoardMaskToBits(gameReplayBoard);
        var boardMaskLong = EncodeBoardMaskIntoLong(boardMask.boardMask);
        var pieceMask = _pieceEncoder.Encode4BitPiecesIntoGuid(boardMask.pieces);

        return (boardMaskLong, pieceMask);
    }

    private (BitArray boardMask, List<byte> pieces) EncodeBoardMaskToBits(LocatedItem<ChessPieceEntity>[,] gameReplayBoard)
    {
        var boardMaskBits = new BitArray(64);
        var idx = 0;
        var pieces = new List<byte>();

        // Go top to bottom, right to left
        for (var file = 7; file >= 0; file--)
        {
            for (var rank = 0; rank <= 7; rank++)
            {
                var piece = gameReplayBoard[rank, file];
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract - Chess Library needs updating to use nullability contracts correctly
                boardMaskBits.Set(idx++, piece != null);

                if (piece != null)
                {
                    var pieceValue = _pieceEncoder.EncodePieceInto4Bits(piece.Item);
                    pieces.Add(pieceValue);
                }
            }
        }

        return (boardMaskBits, pieces);
    }

    private static long EncodeBoardMaskIntoLong(BitArray boardMaskBits)
    {
        if (boardMaskBits.Count != 64)
        {
            throw new ArgumentException("Exactly 64 bits are required to represent a board mask", nameof(boardMaskBits));
        }
        var array = new byte[8];
        boardMaskBits.CopyTo(array, 0);
        return BitConverter.ToInt64(array, 0);
    }
}