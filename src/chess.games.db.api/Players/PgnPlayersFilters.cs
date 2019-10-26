namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PgnPlayersFilters
    {
        public string Name { get; set; }

        public bool IsEmpty => string.IsNullOrEmpty(Name);
    }
}