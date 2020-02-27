
namespace chess.games.db.api.Services
{
    public class StatusIndicatorBuilder
    {
        private static string _indicatorsType1 = @"|/-\";
        private static string _indicatorsType2 = @"·ooOOoo·";

        private readonly string _indicators = _indicatorsType2;
        private int _indicatorIndex = 0;
        private readonly int _indicatorsCount;
        private string _clearPrevious = "";

        public string ClearLast() => _clearPrevious + new string(' ', _clearPrevious.Length) + _clearPrevious;

        public StatusIndicatorBuilder()
        {
            _indicatorsCount = _indicators.Length;
        }
        public string Next()
        {
            var ind = _indicators[_indicatorIndex++].ToString();
            var result = _clearPrevious + ind;

            _clearPrevious = new string('\b', ind.Length);

            if (_indicatorIndex + 1 > _indicatorsCount)
                _indicatorIndex = 0;

            return result;
        }
    }

    public enum IndicatorType
    {
        WrapChar
    }
}