using Microsoft.EntityFrameworkCore;

namespace Chess.Games.Data.Entities;

[PrimaryKey("Id")]
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}