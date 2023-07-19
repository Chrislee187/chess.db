using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;
using Chess.Games.Data.Services;
using Chess.Games.Data.Unit.Tests.Fakers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Shouldly;

namespace Chess.Games.Data.Unit.Tests
{
    public class EventIndexingServiceTests
    {
        private readonly IEventIndexingService _service;
        private readonly Mock<IEventRepository> _eventRepositoryMock;

        public EventIndexingServiceTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _eventRepositoryMock
                .Setup(r => r.Get(null))
                .Returns(new List<EventEntity>() { new EventEntityFaker().Generate() }.AsQueryable());
            _service = new EventIndexingService(_eventRepositoryMock.Object);
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
            var eventText = Guid.NewGuid().ToString();
            _service.TryAdd(eventText);
            _service.TryAdd(eventText);

            _eventRepositoryMock.Verify(r => r.Get(null), Times.Once);
        }

        [Fact]
        public void TryAdd_Should_add_new_entry()
        {
            var eventText = Guid.NewGuid().ToString();
            var e1 = _service.TryAdd(eventText);

            _eventRepositoryMock
                .Verify(r => 
                    r.Add(It.Is<EventEntity>(e => e.Name == eventText)));
        }

        [Fact]
        public void TryAdd_Should_return_existing_entry()
        {
            var eventText = Guid.NewGuid().ToString();
            var e1 = _service.TryAdd(eventText);
            var e2 = _service.TryAdd(eventText);
            e1.ShouldBe(e2);
        }
    }
}