using System;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;
using AutoMapper;
using chess.db.webapi.Models;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api.Players;
using chess.games.db.api.Repositories;
using chess.games.db.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ResourceControllerBase<PlayerDto, Player, Guid>
    {
        private const string GetPlayerRouteName = "GetPlayer";
        private const string GetPlayersRouteName = "GetPlayers";
        private const string CreatePlayerRouteName = "CreatePlayer";
        private const string UpsertPlayerRouteName = "UpsertPlayer";
        private const string PatchPlayerRouteName = "PatchPlayer";
        private const string DeletePlayerRouteName = "DeletePlayer";

        public PlayersController(IMapper mapper,
            IPlayersRepository playersRepository,
            IOrderByPropertyMappingService<PlayerDto, Player> orderByPropertyMappingService,
            IEntityUpdater<Player, Guid> entityUpdater)
            : base(mapper, playersRepository, entityUpdater, orderByPropertyMappingService)
        {
        }
        
        [HttpGet(Name = GetPlayersRouteName)]
        [HttpHead]
        public IActionResult GetPlayers([FromQuery] GetPlayersFilters filters)
        {
            return ResourcesGet(
                filters, 
                Mapper.Map<GetPlayersEntityFilter>(filters), 
                new GetPlayersEntitySearch());
        }

        [HttpGet("{id}", Name = GetPlayerRouteName)]
        public IActionResult GetPlayer(Guid id)
            => ResourceGet(id);

        [HttpPost(Name = CreatePlayerRouteName)]
        public IActionResult CreatePlayer(
            [FromBody] PlayerCreationDto model)
            => ResourceCreate(model);

        [HttpPut("{id}", Name = UpsertPlayerRouteName)]
        public ActionResult UpsertPlayer([FromRoute] Guid id, [FromBody] PlayerUpdateDto model)
            => ResourceUpsert(id, model);

        [HttpPatch("{id}", Name = PatchPlayerRouteName)]
        public ActionResult PatchPlayer([FromRoute] Guid id,
            [FromBody]JsonPatchDocument<PlayerDto> patchDocument)
            => ResourcePatch(id, patchDocument);

        [HttpDelete("{id}", Name= DeletePlayerRouteName)]
        public ActionResult DeletePlayer([FromRoute]Guid id)
            => ResourceDelete(id);
    }
}
