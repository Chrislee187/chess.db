using PgnReader;

namespace chess.games.db.pgnimporter.Extensions
{
    public static class PgnDateExtensions
    {

        public static string RevertDateToText(this PgnDate date)
        {
            return $"{DefaultDateValue(date.Year, "????")}." +
                   $"{DefaultDateValue(date.Month, "??")}." +
                   $"{DefaultDateValue(date.Day, "??")}";
        }

        private static string DefaultDateValue(int? v, string def)
        {
            if (v.HasValue && v != 0)
            {
                return v.ToString();
            }

            return def;
        }
    }

}