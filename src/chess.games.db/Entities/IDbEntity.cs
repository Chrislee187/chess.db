using System;

namespace chess.games.db.Entities
{
    public interface IDbEntity
    {
        Guid Id { get; set; }
    }
}