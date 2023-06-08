using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;
using Chess.Games.Data.Services;
using Chess.Games.Data.Unit.Tests.Fakers;
using Moq;
using Shouldly;

namespace Chess.Games.Data.Unit.Tests;

public class PlayerIndexingServiceTests
{
    private readonly IPlayerIndexingService _service;
    private readonly Mock<IPlayerRepository> _eventRepositoryMock;

    public PlayerIndexingServiceTests()
    {
        _eventRepositoryMock = new Mock<IPlayerRepository>();
        _eventRepositoryMock
            .Setup(r => r.Get(null))
            .Returns(new List<PlayerEntity>() { new PlayerEntityFaker().Generate() }.AsQueryable());
        _service = new PlayerIndexingService(_eventRepositoryMock.Object);
    }

    [Fact]
    public void TryAdd_Should_get_index_from_repo_on_first_access()
    {
        _service.TryAdd(Guid.NewGuid().ToString());

        _eventRepositoryMock.Verify(r => r.Get(null), Times.Once);
    }

    [Fact]
    public void TryAdd_Should_not_get_index_from_repo_on_subsequent_access()
    {
        _service.TryAdd(Guid.NewGuid().ToString());
        _service.TryAdd(Guid.NewGuid().ToString());

        _eventRepositoryMock.Verify(r => r.Get(null), Times.Once);
    }

    [Fact]
    public void TryAdd_Should_add_new_entry()
    {
        var playerText = Guid.NewGuid().ToString();
        var e1 = _service.TryAdd(playerText);

        _eventRepositoryMock
            .Verify(r =>
                r.Add(It.Is<PlayerEntity>(e => e.Name == playerText)));
    }

    [Fact]
    public void TryAdd_Should_return_existing_entry()
    {
        var playerText = Guid.NewGuid().ToString();
        var e1 = _service.TryAdd(playerText);
        var e2 = _service.TryAdd(playerText);
        e1.ShouldBe(e2);
    }
}