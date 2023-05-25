namespace Chess.Games.Data.Entities;

public class GameEntity : Entity
{
    public string SourceEventText { get; set; }
    public EventEntity EventEntity { get; set; }
    public Guid EventId { get; set; }

    public string SourceSiteText { get; set; }
    public SiteEntity SiteEntity { get; set; }
    public Guid SiteId { get; set; }

    public string SourceWhitePlayerText{ get; set; }
    public PlayerEntity White { get; set; }
    public Guid WhiteId { get; set; }

    public string SourceBlackPlayerText { get; set; }
    public PlayerEntity Black { get; set; }
    public Guid BlackId { get; set; }
    
    public DateTimeOffset Date { get; set; }
    public int Round { get; set; }
    public GameResult Result { get; set; }

    public string SourceMoveText { get; set; }
}