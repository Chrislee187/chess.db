using chess.engine.Entities;
using chess.engine.Game;

namespace Chess.Data.PGNImporter;

public class ChessPiece4BitEncoder
{
    public byte EncodePieceInto4Bits(ChessPieceEntity chessPieceEntity)
    {
        var piece = chessPieceEntity.Piece;
        byte pieceValue = piece switch
        {
            ChessPieceName.Pawn => 1,
            ChessPieceName.Rook => 2,
            ChessPieceName.Bishop => 3,
            ChessPieceName.Knight => 4,
            ChessPieceName.Queen => 5,
            ChessPieceName.King => 6,
            _ => throw new ArgumentOutOfRangeException(nameof(chessPieceEntity))
        };
        if (chessPieceEntity.Player == Colours.Black)
        {
            pieceValue += 8;
        }

        return pieceValue;
    }

    public Guid Encode4BitPiecesIntoGuid(IEnumerable<byte> pieces)
    {
        var paddedPieces = new List<byte>(pieces);

        switch (paddedPieces.Count)
        {
            case > 32:
                throw new ArgumentOutOfRangeException(nameof(paddedPieces),
                    "A maximum of 32 pieces are allowed for Guid Pieces encoding");
            case < 32:
            {
                var padding = new byte[32 - paddedPieces.Count];
                paddedPieces.AddRange(padding);
                break;
            }
        }

        var malformedGuid = paddedPieces.Aggregate("", (s, a) => s + a.ToString("x"));

        var pseudoGuid = malformedGuid[0..8] + "-"
                                             + malformedGuid[8..12] + "-"
                                             + malformedGuid[12..16] + "-"
                                             + malformedGuid[16..20] + "-"
                                             + malformedGuid[20..];
        return Guid.Parse(pseudoGuid);
    }
}