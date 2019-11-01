namespace chess.games.db.Entities
{
    public class Site : DbEntity, IHaveAName
    {
        public string Name { get; set; }
    }
}