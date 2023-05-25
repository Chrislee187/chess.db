using PgnReader;

namespace Chess.PGNImporter;

internal class PgonGame
{
    public string Event { get; set; }
    public string Site { get; }
    public DateTimeOffset Date { get; }
    public int Round { get; }
    public string White { get; }
    public string Black { get; }
    public PgnGameResult Result { get; }
    public string ShortAlgebraicNotation { get; init; }
    public string MoveTextKey { get; init; }
    public string OriginalPgn { get; }
    public string? LongAlgebraicNotation { get; } = null;
    public Guid? EventUid { get; set; } = null;

    public PgonGame(PgnGame pgnGame)
    {
        Event = pgnGame.Event;
        Site = pgnGame.Site;
        Date = new DateTimeOffset(new DateTime(pgnGame.Date.Year ?? 0, pgnGame.Date.Month ?? 0, pgnGame.Date.Day ?? 0));

        if (int.TryParse(pgnGame.Round, out var r))
        {
            Round = r;
        }

        White = pgnGame.White;
        Black = pgnGame.Black;
        Result = pgnGame.Result;
        ShortAlgebraicNotation = pgnGame.MoveText;
        OriginalPgn = pgnGame.Round;
        MoveTextKey = pgnGame.MoveText;
    }
}