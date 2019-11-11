namespace chess.games.db.Entities
{
    public interface IDbEntity<T>
    {
        T Id { get; set; }
    }
}