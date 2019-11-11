namespace chess.games.db.Entities
{
    public abstract class DbEntity<T> : IDbEntity<T>
    {
        public T Id { get; set; }
    }
}