using EasyEF.Entities;

namespace Chess.Games.Data.Entities;

public record GameEntity : Entity
{
    public string SourceEventText { get; set; }
    public EventEntity Event { get; set; }
    public Guid EventId { get; set; }

    public string SourceSiteText { get; set; }
    public SiteEntity Site { get; set; }
    public Guid SiteId { get; set; }

    public string SourceWhitePlayerText{ get; set; }
    public PlayerEntity White { get; set; }
    public Guid WhiteId { get; set; }

    public string SourceBlackPlayerText { get; set; }
    public PlayerEntity Black { get; set; }
    public Guid BlackId { get; set; }
    
    public DateTime Date { get; set; }
    public int Round { get; set; }
    public GameResult Result { get; set; }

    public string SourceMoveText { get; set; }
    public string LanMoveText { get; set; }
    public int LanMoveTextHash { get; set; }
}