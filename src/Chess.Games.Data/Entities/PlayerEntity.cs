using EasyEF.Entities;

namespace Chess.Games.Data.Entities;

public record PlayerEntity : Entity
{
    public string Name { get; set; } = null!;
}