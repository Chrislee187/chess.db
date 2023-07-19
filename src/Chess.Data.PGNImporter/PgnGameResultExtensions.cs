using PgnReader;

namespace Chess.PGNImporter.Extensions
{
    public static class PgnGameResultExtensions
    {
        public static string RevertToText(this PgnGameResult pgnR)
        {
            switch (pgnR)
            {
                case PgnGameResult.Unknown:
                    return "? ?";
                case PgnGameResult.Draw:
                    return "1/2-1/2";
                case PgnGameResult.WhiteWins:
                    return "1-0";
                case PgnGameResult.BlackWins:
                    return "0-1";
                default:
                    throw new ArgumentOutOfRangeException(nameof(pgnR), pgnR, null);
            }
        }
    }
}