using Bogus;
using Chess.Games.Data.Entities;

namespace Chess.Games.Data.Unit.Tests.Fakers;

public class SiteEntityFaker : Faker<SiteEntity>
{
    public SiteEntityFaker()
    {
        base.RuleFor(e => e.Name, f => f.Address.City());
    }
}