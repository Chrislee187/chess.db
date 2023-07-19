using EasyEF.Entities;

namespace Chess.Games.Data.Entities;

public record SiteEntity : Entity
{
    public string Name { get; set; } = null!;
}