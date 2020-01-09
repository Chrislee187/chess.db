using System;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;
using AutoMapper;
using chess.db.webapi.Models;
using chess.games.db.api.Repositories;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Mvc;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ResourceControllerBase<EventDto, Event, Guid>
    {
        private const string EventRouteName = "GetEvent";
        private const string EventsRouteName = "GetEvents";

        public EventsController(IMapper mapper,
            IEventsRepository gamesRepository,
            IOrderByPropertyMappingService<EventDto, Event> orderByPropertyMappingService,
            IEntityUpdater<Event, Guid> entityUpdater)
            : base(mapper, gamesRepository, entityUpdater, orderByPropertyMappingService)
        {
        }

        [HttpGet(Name = EventsRouteName)]
        [HttpHead]
        public IActionResult Events()
        {
            return ResourcesGet();
        }

        [HttpGet("{id}", Name = EventRouteName)]
        public IActionResult Event(Guid id)
            => ResourceGet(id);
    }
}
