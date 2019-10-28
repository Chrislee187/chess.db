using System;
using System.Collections.Generic;
using System.Linq;
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
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PlayersController> _logger;
        private readonly IPlayersRepository _playersRepository;

        private const string GetPlayerRouteName = "GetPlayer";

        public PlayersController(
            IMapper mapper,
            IPlayersRepository playersRepository,
            ILogger<PlayersController> logger
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _playersRepository = playersRepository ?? throw new ArgumentNullException(nameof(playersRepository));
            _logger = logger ?? NullLogger<PlayersController>.Instance;
        }

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<PlayerDto>> GetPlayers(
            [FromQuery] PlayerResourceParameters parameters
        )
        {
            var filters = _mapper.Map<PlayersFilters>(parameters);
            var query = _mapper.Map<PlayersSearchQuery>(parameters);

            var players = _playersRepository
                .GetPlayers(filters, query)
                .Take(1000); // TODO: Temp restriction until paging is implemented

            return Ok(_mapper.Map<IEnumerable<PlayerDto>>(players));
        }

        [HttpGet("{id}", Name = GetPlayerRouteName)]
        public ActionResult<PlayerDto> GetPlayer(Guid id)
        {
            var player = _playersRepository.GetPlayer(id);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PlayerDto>(player));
        }

        [HttpPost]
        public ActionResult<PlayerDto> CreatePlayer(PlayerCreationDto model)
        {
            var entity = _mapper.Map<Player>(model);

            _playersRepository.Add(entity);
            _playersRepository.Save();

            var createdPlayer = _mapper.Map<PlayerDto>(entity);
            
            // NOTE: CreatedAtRoute returns 201 (Created) and places a 'Location' entry in
            // the response header containing the uri to retrieve the newly added resource
            return CreatedAtRoute(
                GetPlayerRouteName,
                new {createdPlayer.Id},
                createdPlayer);
        }
    }
}
