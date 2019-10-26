using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using chess.db.webapi.Models;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api.Players;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PlayersController> _logger;
        private readonly IPlayersRepository _playersRepository;

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

        /// <summary>
        /// PgnPlayers are created by the import mechanism and are not creatable, editable or deletable, by the API.
        /// They represent the original data from the original source before attempting to dedupe the data
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<PgnPlayerDto>> GetPgnPlayers(
            [FromQuery] PgnPlayerResourceParameters parameters
            )
        {
            var filters = _mapper.Map<PgnPlayersFilters>(parameters);
            var query = _mapper.Map<PgnPlayersSearchQuery>(parameters);

            var players = _playersRepository
                .GetPgnPlayers(filters, query)
                .Take(1000); // TODO: Temp restriction until paging is implemented

            return Ok(_mapper.Map<IEnumerable<PgnPlayerDto>>(players));
        }

        [HttpGet("{playerId}")]
        public ActionResult<PgnPlayerDto> GetPgnPlayer(Guid playerId)
        {
            var player = _playersRepository.GetPlayer(playerId);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PgnPlayerDto>(player));
        }
    }
}