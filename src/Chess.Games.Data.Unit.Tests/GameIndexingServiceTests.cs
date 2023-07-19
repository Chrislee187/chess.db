using System.Linq.Expressions;
using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;
using Chess.Games.Data.Services;
using Chess.Games.Data.Unit.Tests.Fakers;
using Moq;
using Shouldly;

namespace Chess.Games.Data.Unit.Tests;

public class GameIndexingServiceTests
{
    private readonly IGameIndexingService _service;
    private readonly Mock<IGameRepository> _repositoryMock;

    public GameIndexingServiceTests()
    {
        _repositoryMock = new Mock<IGameRepository>();
        _repositoryMock
            .Setup(r => r.Get(null))
            .Returns(new List<GameEntity>() { new GameEntityFaker().Generate() }.AsQueryable());
        _service = new GameIndexingService(_repositoryMock.Object);
    }

    [Fact]
    public void TryAdd_Should_get_from_repo_to_test_uniqueness()
    {
        _service.TryAdd(new GameEntityFaker().Generate(), out var existing);

        _repositoryMock.Verify(r => r.FirstOrDefault(It.IsAny<Expression<Func<GameEntity, bool>>>()), Times.Once);
    }

    [Fact]
    public void TryAdd_Should_add_new_entry()
    {
        var game = new GameEntityFaker().Generate();
        var e1 = _service.TryAdd(game, out var existing);
        
        e1.ShouldBeTrue();
        existing.ShouldBeNull();

        _repositoryMock
            .Verify(r =>
                r.Add(It.Is<GameEntity>(e => e.SourceWhitePlayerText == game.SourceWhitePlayerText)));
    }

    [Fact]
    public void TryAdd_Should_return_existing_entry()
    {
        var game = new GameEntityFaker().Generate();
        var e1 = _service.TryAdd(game, out var existing);
        e1.ShouldBeTrue();
        existing.ShouldBeNull();

        _repositoryMock
            .Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<GameEntity, bool>>>()))
            .Returns(game);


        var e2 = _service.TryAdd(game, out existing);
        e2.ShouldBeFalse();
        existing.ShouldBe(game);
    }
}