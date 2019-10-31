using System;

namespace chess.games.db.Entities
{
    public interface IDbEntity
    {
        Guid Id { get; set; }
    }

    public abstract class DbEntity : IDbEntity
    {
        public Guid Id { get; set; }
    }
}