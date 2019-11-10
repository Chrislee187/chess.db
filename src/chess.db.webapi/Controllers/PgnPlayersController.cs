using System;
using System.Net;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Parameters;
using AutoMapper;
using chess.db.webapi.Models;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api.PgnPlayers;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Mvc;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/pgnplayers")]
    public class PgnPlayersController : ResourceControllerBase<PgnPlayerDto, PgnPlayer>
    {
        private readonly IPgnPlayersRepository _pgnPlayersRepository;

        private const string GetPgnPlayersRouteName = "GetPgnPlayers";
        private const string GetPgnPlayerRouteName = "GetPgnPlayer";
        private const string GetPgnPlayerByIdRouteName = "GetPgnPlayerById";
        public PgnPlayersController(IMapper mapper,
            IPgnPlayersRepository pgnPlayersRepository,
            IOrderByPropertyMappingService<PgnPlayerDto, PgnPlayer> orderByPropertyMappingService,
            IEntityUpdater<PgnPlayer> entityUpdater)
                : base(mapper, pgnPlayersRepository, orderByPropertyMappingService, entityUpdater)
        {
             _pgnPlayersRepository = NullX.Throw(pgnPlayersRepository, nameof(pgnPlayersRepository));

            HateoasConfig.ResourceGetRouteName.Set(GetPgnPlayerRouteName);
            HateoasConfig.ResourcesGetRouteName.Set(GetPgnPlayersRouteName);
        }
        
        [HttpGet(Name = GetPgnPlayersRouteName)]
        [HttpHead]
        public IActionResult GetPgnPlayers(
            [FromQuery] GetPgnPlayersParameters parameters
            )
        {
            var filters = Mapper.Map<GetPgnPlayersFilters>(parameters);
            var query = Mapper.Map<GetPgnPlayersSearchQuery>(parameters);

            return ResourcesGet(
                parameters,
                filters,
                query);
        }

        [HttpGet("{id:Guid}", Name = GetPgnPlayerByIdRouteName)]
        public ActionResult<PgnPlayerDto> GetPgnPlayer(Guid id)
            => Problem(
                "PgnPlayers are referenced by name, not Id.", 
                Url.Link(GetPgnPlayerByIdRouteName, new {id}),
                (int) HttpStatusCode.Forbidden,
                $"Invalid route parameter {id}"
                );

        [HttpGet("{name}", Name=GetPgnPlayerRouteName)]
        public ActionResult<PgnPlayerDto> GetPgnPlayer(string name)
        {
            var player = _pgnPlayersRepository.Get(name);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<PgnPlayerDto>(player));
        }

        [HttpOptions]
        public IActionResult GetOptions()
            => ResourceOptions("OPTIONS,HEAD,GET");
    }
}