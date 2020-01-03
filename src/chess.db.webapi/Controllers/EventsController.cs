using System;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;
using AutoMapper;
using chess.db.webapi.Models;
using chess.games.db.api.Players;
using chess.games.db.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ResourceControllerBase<EventDto, Event, Guid>
    {
        // NOTE: The ResourceControllerBase needs to know the names of the supported
        // routes. By default is assumes names based based on `nameof(TEntity)` that follow the pattern below
        // override with `ResourceControllerBase.HateoasConfig.XXXXRouteName.Set()` calls in the ctor or ctor
        // params if a different convention is required
        private const string GetEventRouteName = "GetEvent";
        private const string GetEventsRouteName = "GetEvents";
        private const string CreateEventRouteName = "CreateEvent";
        private const string UpsertEventRouteName = "UpsertEvent";
        private const string PatchEventRouteName = "PatchEvent";
        private const string DeleteEventRouteName = "DeleteEvent";

        public EventsController(IMapper mapper,
            IEventsRepository gamesRepository,
            IOrderByPropertyMappingService<EventDto, Event> orderByPropertyMappingService,
            IEntityUpdater<Event, Guid> entityUpdater)
            : base(mapper, gamesRepository, entityUpdater, orderByPropertyMappingService)
        {
        }

        [HttpGet(Name = GetEventsRouteName)]
        [HttpHead]
        public IActionResult GetEvents()
        {
            return ResourcesGet<object>(null, null, null);
        }

        [HttpGet("{id}", Name = GetEventRouteName)]
        public IActionResult GetEvent(Guid id)
            => ResourceGet(id);


        [HttpPatch("{id}", Name = PatchEventRouteName)]
        public ActionResult PatchPlayer([FromRoute] Guid id,
            [FromBody]JsonPatchDocument<EventDto> patchDocument)
            => ResourcePatch(id, patchDocument);

        [HttpOptions]
        public IActionResult GetOptions()
            => ResourceOptions();
    }
}
