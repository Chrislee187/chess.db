using Bogus;
using Chess.Games.Data.Entities;

namespace Chess.Games.Data.Unit.Tests.Fakers;

public class EventEntityFaker : Faker<EventEntity>
{
    public EventEntityFaker()
    {
        base.RuleFor(e => e.Name, f => f.Lorem.Word());
    }
}