using System;

namespace chess.games.db.Entities
{
    public abstract class DbEntity : IDbEntity
    {
        public Guid Id { get; set; }
    }
}