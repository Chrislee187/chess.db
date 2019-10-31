using System;
using System.Collections.Generic;
using System.Net;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Parameters;
using AutoMapper;
using chess.db.webapi.Models;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api.Players;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace chess.db.webapi.Controllers
{
    /// <summary>
    /// PgnPlayers are created by the import mechanism and are not creatable or deletable by the API.
    /// They represent the original data from the original source before attempting to dedupe the data
    /// </summary>
    [ApiController]
    [Route("api/pgnplayers")]
    public class PgnPlayersController : ResourceControllerBase<PgnPlayerDto, PgnPlayer>
    {

        private readonly ILogger<PgnPlayersController> _logger;
        private readonly IPgnPlayersRepository _pgnPlayersRepository;

        private const string GetPgnPlayersRouteName = "GetPgnPlayers";
        private const string GetPgnPlayerRouteName = "GetPgnPlayer";
        private const string GetPgnPlayerByIdRouteName = "GetPgnPlayerById";
        public PgnPlayersController(IMapper mapper,
            IPgnPlayersRepository pgnPlayersRepository,
            IOrderByPropertyMappingService<PgnPlayerDto, PgnPlayer> orderByPropertyMappingService,
            ILogger<PgnPlayersController> logger)
                : base(mapper, pgnPlayersRepository, orderByPropertyMappingService)
        {
             _pgnPlayersRepository = pgnPlayersRepository ?? throw new ArgumentNullException(nameof(pgnPlayersRepository));
            _logger = logger ?? NullLogger<PgnPlayersController>.Instance;
        }


        [HttpGet(Name = GetPgnPlayersRouteName)]
        [HttpHead]
        public ActionResult<IEnumerable<PgnPlayerDto>> GetPgnPlayers(
            [FromQuery] GetPgnPlayersParameters parameters
            )
        {
            var filters = Mapper.Map<GetPgnPlayersFilters>(parameters);
            var query = Mapper.Map<GetPgnPlayersSearchQuery>(parameters);

            return ResourcesGet(
                parameters,
                filters,
                query,
                GetPgnPlayersRouteName);
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