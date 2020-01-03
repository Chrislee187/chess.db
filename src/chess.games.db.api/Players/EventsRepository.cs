using System;
using AspNetCore.MVC.RESTful.Repositories;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IEventsRepository : IResourceRepository<Event, Guid>
    {

    }
    public class EventsRepository : EntityFrameworkResourceRepository<Event, Guid>, IEventsRepository
    {
        public EventsRepository(
            ChessGamesDbContext dbContext
        )
            : base(dbContext) { }

    }
}