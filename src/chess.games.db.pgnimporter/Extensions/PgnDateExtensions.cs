using PgnReader;

namespace chess.games.db.pgnimporter.Extensions
{
    public static class PgnDateExtensions
    {

        public static string RevertDateToText(this PgnDate date)
        {
            return $"{FixValue(date.Year, "????")}." +
                   $"{FixValue(date.Month, "??")}." +
                   $"{FixValue(date.Day, "??")}";
        }

        private static string FixValue(int? v, string def)
        {
            if (v.HasValue && v != 0)
            {
                return v.ToString();
            }

            return def;
        }
    }
}