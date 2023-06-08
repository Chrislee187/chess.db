using Bogus;
using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;
using EasyEF.Entities;
using EasyEF.Repos;
using Moq;

namespace Chess.Games.Data.Unit.Tests.Fakers;

public class GameEntityFaker : Faker<GameEntity>
{
    public GameEntityFaker()
    {
        base.RuleFor(e => e.SourceWhitePlayerText, _ => new PlayerEntityFaker().Generate().Name);
        base.RuleFor(e => e.SourceBlackPlayerText, _ => new PlayerEntityFaker().Generate().Name);
        base.RuleFor(e => e.SourceEventText, _ => new EventEntityFaker().Generate().Name);
        base.RuleFor(e => e.SourceSiteText, _ => new SiteEntityFaker().Generate().Name);
        base.RuleFor(e => e.SourceMoveText, _ => Guid.NewGuid().ToString());
        base.RuleFor(e => e.Round, f => f.Random.Int(1,50));
    }
}
