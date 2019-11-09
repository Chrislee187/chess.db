using System;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Parameters;
using AutoMapper;
using chess.db.webapi.Models;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api.Players;
using chess.games.db.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ResourceControllerBase<PlayerDto, Player>
    {
        private readonly ILogger<PlayersController> _logger;

        private const string GetPlayerRouteName = "GetPlayer";
        private const string GetPlayersRouteName = "GetPlayers";
        private const string CreatePlayerRouteName = "CreatePlayer";
        private const string UpsertPlayerRouteName = "UpsertPlayer";
        private const string PatchPlayerRouteName = "PatchPlayer";
        private const string DeletePlayerRouteName = "DeletePlayer";

        public PlayersController(IMapper mapper,
            IPlayersRepository playersRepository,
            IOrderByPropertyMappingService<PlayerDto, Player> orderByPropertyMappingService,
            IEntityUpdater<Player> entityUpdater,
            ILogger<PlayersController> logger)
            : base(mapper, playersRepository, orderByPropertyMappingService, entityUpdater)
        {
            _logger = logger ?? NullLogger<PlayersController>.Instance;

            HateoasConfig.ResourcesGetRouteName.Set(GetPlayersRouteName);
            HateoasConfig.ResourceGetRouteName.Set(GetPlayerRouteName);
            HateoasConfig.ResourceCreateRouteName.Set(CreatePlayerRouteName);
            HateoasConfig.ResourceUpsertRouteName.Set(UpsertPlayerRouteName);
            HateoasConfig.ResourcePatchRouteName.Set(PatchPlayerRouteName);
            HateoasConfig.ResourceDeleteRouteName.Set(DeletePlayerRouteName);
        }


        [HttpGet(Name = GetPlayersRouteName)]
        [HttpHead]
        public IActionResult GetPlayers([FromQuery] GetPlayersParameters parameters)
        {

            var resourcesGet = ResourcesGet(
                parameters, 
                Mapper.Map<GetPlayersFilters>(parameters), 
                Mapper.Map<GetPlayersSearchQuery>(parameters));

            return resourcesGet;
        }

        [HttpGet("{id}", Name = GetPlayerRouteName)]
        public ActionResult<PlayerDto> GetPlayer(Guid id, string shape)
            => ResourceGet(id, shape);

        [HttpPost(Name = CreatePlayerRouteName)]
        public ActionResult<PlayerDto> CreatePlayer([FromBody] PlayerCreationDto model)
            => ResourceCreate(model);

        [HttpPut("{id}", Name = UpsertPlayerRouteName)]
        public ActionResult UpsertPlayer([FromRoute] Guid id, [FromBody] PlayerUpdateDto model)
            => ResourceUpsert(id, model);

        [HttpPatch("{id}", Name = PatchPlayerRouteName)]
        public ActionResult PatchPlayer([FromRoute] Guid id,
            [FromBody]JsonPatchDocument<PlayerUpdateDto> patchDocument)
            => ResourcePatch(id, patchDocument);

        [HttpDelete("{id}", Name= DeletePlayerRouteName)]
        public ActionResult DeletePlayer([FromRoute]Guid id)
            => ResourceDelete(id);

        [HttpOptions]
        public IActionResult GetOptions()
            => ResourceOptions("HEAD", "OPTIONS", "GET", "POST", "PUT", "PATCH", "DELETE");
    }
}
