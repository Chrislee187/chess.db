using Bogus;
using Chess.Games.Data.Entities;

namespace Chess.Games.Data.Unit.Tests.Fakers;

public class PlayerEntityFaker : Faker<PlayerEntity>
{
    public PlayerEntityFaker()
    {
        base.RuleFor(e => e.Name, f => f.Name.FullName());
    }
}