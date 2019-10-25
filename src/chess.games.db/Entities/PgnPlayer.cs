namespace chess.games.db.Entities
{
    public class PgnPlayer : DbEntity, IHaveAName
    {
        public string Name { get; set; }
    }
}