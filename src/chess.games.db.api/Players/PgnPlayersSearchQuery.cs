namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PgnPlayersSearchQuery
    {
        public string QueryText { get; set; }

        public bool IsEmpty => string.IsNullOrEmpty(QueryText);
    }
}