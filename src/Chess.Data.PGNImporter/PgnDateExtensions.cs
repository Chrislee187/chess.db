using PgnReader;

namespace Chess.Data.PGNImporter
{
    public static class PgnDateExtensions
    {

        public static string RevertDateToText(this PgnDate date)
        {
            return $"{DefaultDateValue(date.Year, "????")}." +
                   $"{DefaultDateValue(date.Month, "??")}." +
                   $"{DefaultDateValue(date.Day, "??")}";
        }
        public static DateTime ToDateTime(this PgnDate date) => 
            date is { Day: not null, Month: not null, Year: not null } 
                ? new DateTime(date.Year.Value, date.Month.Value, date.Day.Value) 
                : DateTime.MinValue;

        private static string DefaultDateValue(int? v, string def)
        {
            if (v.HasValue && v.Value != 0)
            {
                return v.ToString()!;
            }

            return def;
        }
    }

}