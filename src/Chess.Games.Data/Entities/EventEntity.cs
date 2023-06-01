using EasyEF.Entities;

namespace Chess.Games.Data.Entities;

public record EventEntity : Entity
{
    public string Name { get; set; } = null!;
}