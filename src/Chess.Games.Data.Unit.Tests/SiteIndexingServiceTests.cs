using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;
using Chess.Games.Data.Services;
using Chess.Games.Data.Unit.Tests.Fakers;
using Moq;
using Shouldly;

namespace Chess.Games.Data.Unit.Tests;

public class SiteIndexingServiceTests
{
    private readonly ISiteIndexingService _service;
    private readonly Mock<ISiteRepository> _eventRepositoryMock;

    public SiteIndexingServiceTests()
    {
        _eventRepositoryMock = new Mock<ISiteRepository>();
        _eventRepositoryMock
            .Setup(r => r.Get(null))
            .Returns(new List<SiteEntity>() { new SiteEntityFaker().Generate() }.AsQueryable());
        _service = new SiteIndexingService(_eventRepositoryMock.Object);
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
        var siteText = Guid.NewGuid().ToString();
        _service.TryAdd(siteText);
        _service.TryAdd(siteText);

        _eventRepositoryMock.Verify(r => r.Get(null), Times.Once);
    }

    [Fact]
    public void TryAdd_Should_add_new_entry()
    {
        var siteText = Guid.NewGuid().ToString();
        _service.TryAdd(siteText);

        _eventRepositoryMock
            .Verify(r =>
                r.Add(It.Is<SiteEntity>(e => e.Name == siteText)));
    }

    [Fact]
    public void TryAdd_Should_return_existing_entry()
    {
        var siteText = Guid.NewGuid().ToString();
        var e1 = _service.TryAdd(siteText);
        var e2 = _service.TryAdd(siteText);
        e1.ShouldBe(e2);
    }
}