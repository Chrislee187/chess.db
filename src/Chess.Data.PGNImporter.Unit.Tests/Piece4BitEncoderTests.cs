using chess.engine.Entities;
using chess.engine.Game;
using Shouldly;

namespace Chess.Data.PGNImporter.Unit.Tests
{
    public class Piece4BitEncoderTests
    {
        private readonly ChessPiece4BitEncoder _encoder;
        private readonly List<ChessPieceEntity> _startingPieces = new()
        {
            new RookEntity(Colours.Black),
            new KnightEntity(Colours.Black),
            new BishopEntity(Colours.Black),
            new KingEntity(Colours.Black),
            new QueenEntity(Colours.Black),
            new BishopEntity(Colours.Black),
            new KnightEntity(Colours.Black),
            new RookEntity(Colours.Black),
            new PawnEntity(Colours.Black),
            new PawnEntity(Colours.Black),
            new PawnEntity(Colours.Black),
            new PawnEntity(Colours.Black),
            new PawnEntity(Colours.Black),
            new PawnEntity(Colours.Black),
            new PawnEntity(Colours.Black),
            new PawnEntity(Colours.Black),
            new PawnEntity(Colours.White),
            new PawnEntity(Colours.White),
            new PawnEntity(Colours.White),
            new PawnEntity(Colours.White),
            new PawnEntity(Colours.White),
            new PawnEntity(Colours.White),
            new PawnEntity(Colours.White),
            new PawnEntity(Colours.White),
            new RookEntity(Colours.White),
            new KnightEntity(Colours.White),
            new BishopEntity(Colours.White),
            new KingEntity(Colours.White),
            new QueenEntity(Colours.White),
            new BishopEntity(Colours.White),
            new KnightEntity(Colours.White),
            new RookEntity(Colours.White),
        };

        private readonly Guid _startingPieceMaskGuid = new("acbedbca-9999-9999-1111-111124365342");
        private readonly ChessPieceEntityFactory _chessPieceEntityFactory = new();

        public Piece4BitEncoderTests()
        {
            _encoder = new ChessPiece4BitEncoder();
        }

        [Theory]
        [InlineData(ChessPieceName.Pawn, 1)]
        [InlineData(ChessPieceName.Rook, 2)]
        [InlineData(ChessPieceName.Bishop, 3)]
        [InlineData(ChessPieceName.Knight, 4)]
        [InlineData(ChessPieceName.Queen, 5)]
        [InlineData(ChessPieceName.King, 6)]
        public void ShouldEncodeWhitePieces(ChessPieceName pieceName, byte expectedValue)
        {
            var piece = _chessPieceEntityFactory.Create(pieceName, Colours.White);

            _encoder.EncodePieceInto4Bits(piece).ShouldBe(expectedValue);
        }

        [Theory]
        [InlineData(ChessPieceName.Pawn, 9)]
        [InlineData(ChessPieceName.Rook, 10)]
        [InlineData(ChessPieceName.Bishop, 11)]
        [InlineData(ChessPieceName.Knight, 12)]
        [InlineData(ChessPieceName.Queen, 13)]
        [InlineData(ChessPieceName.King, 14)]
        public void ShouldEncodeBlackPieces(ChessPieceName pieceName, byte expectedValue)
        {
            var piece = _chessPieceEntityFactory.Create(pieceName, Colours.Black);

            _encoder.EncodePieceInto4Bits(piece).ShouldBe(expectedValue);
        }

        [Fact]
        public void Should_encoding_starting_board_pieces_to_guid()
        {
            var startingPieces4Bit = _startingPieces.Select(_encoder.EncodePieceInto4Bits);

            var encode4BitPiecesIntoGuid = _encoder.Encode4BitPiecesIntoGuid(startingPieces4Bit);

            encode4BitPiecesIntoGuid.ShouldBe(_startingPieceMaskGuid);
        }

        [Fact]
        public void Should_throw_if_too_many_pieces_to_encode()
        {
            var tooManyPieces = Enumerable.Range(0, 33)
                .Select(_ => _encoder.EncodePieceInto4Bits(new PawnEntity(Colours.White)));
            Should.Throw<ArgumentOutOfRangeException>(() => _encoder.Encode4BitPiecesIntoGuid(tooManyPieces));
        }

        [Fact]
        public void Should_pad_with_zero_if_less_than_32_pieces()
        {
            var notEnoughPieces = new [] { _encoder.EncodePieceInto4Bits(new PawnEntity(Colours.White)) } ;
            var piecesGuid = _encoder.Encode4BitPiecesIntoGuid(notEnoughPieces);
            piecesGuid.ShouldBe(new Guid("10000000-0000-0000-0000-000000000000"));
        }
    }
}