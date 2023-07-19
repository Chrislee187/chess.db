using EasyEF.Entities;

namespace Chess.Games.Data.Entities;

public record GameEntity : Entity
{
    public string SourceEventText { get; set; } = null!;
    public EventEntity Event { get; set; } = null!;
    public Guid EventId { get; set; }

    public string SourceSiteText { get; set; } = null!;
    public SiteEntity Site { get; set; } = null!;
    public Guid SiteId { get; set; }

    public string SourceWhitePlayerText{ get; set; } = null!;
    public PlayerEntity White { get; set; } = null!;
    public Guid WhiteId { get; set; }

    public string SourceBlackPlayerText { get; set; } = null!;
    public PlayerEntity Black { get; set; } = null!;
    public Guid BlackId { get; set; }
    
    public DateTime Date { get; set; }
    public int Round { get; set; }
    public GameResult Result { get; set; }

    public string SourceMoveText { get; set; } = null!;
    public string LanMoveText { get; set; } = null!;
    public int LanMoveTextHash { get; set; }
}