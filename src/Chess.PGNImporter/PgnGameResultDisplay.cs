using PgnReader;

namespace Chess.PGNImporter;

public static class PgnGameResultDisplay
{
    private static readonly Dictionary<PgnGameResult, string> _shortStrings = new()
    {
        { PgnGameResult.Unknown, "U" },
        { PgnGameResult.WhiteWins, "W" },
        { PgnGameResult.BlackWins, "B" },
        { PgnGameResult.Draw, "D" },
    };

    public static string ToShortString(this PgnGameResult result) => _shortStrings[result];
    public static string ToEnumString(this PgnGameResult result) => result.ToString();
}