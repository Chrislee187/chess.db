using System;

namespace chess.games.db.Entities
{
    public static class GameResultExtensions
    {
        public static string RevertToText(this GameResult pgnR)
        {
            switch (pgnR)
            {
                case GameResult.Unknown:
                    return "? ?";
                case GameResult.Draw:
                    return "1/2-1/2";
                case GameResult.WhiteWins:
                    return "1-0";
                case GameResult.BlackWins:
                    return "0-1";
                default:
                    throw new ArgumentOutOfRangeException(nameof(pgnR), pgnR, null);
            }
        }
        public static GameResult ToGameResult(this string pgnGameResult)
        {
            switch (pgnGameResult)
            {

                case "1/2-1/2":
                    return GameResult.Draw;
                case "1-0":
                    return GameResult.WhiteWins;
                case "0-1":
                    return GameResult.BlackWins;
                default:
                    return GameResult.Unknown;
            }
        }
    }
}