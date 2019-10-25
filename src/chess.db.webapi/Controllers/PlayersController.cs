using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using chess.db.webapi.Models;
using chess.games.db.api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly ILogger<PlayersController> _logger;
        private readonly IPlayersRepository _playersRepository;
        private readonly IMapper _mapper;

        public PlayersController(
            IPlayersRepository playersRepository,
            IMapper mapper,
            ILogger<PlayersController> logger
            )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _playersRepository = playersRepository ?? throw new ArgumentNullException(nameof(playersRepository));
            _logger = logger ?? NullLogger<PlayersController>.Instance;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<PlayerDto>> GetPlayers()
        {
            var players = _playersRepository.GetPlayers().Take(1000); // TODO: Temp restriction until paging is implemented

            return Ok(_mapper.Map<IEnumerable<PlayerDto>>(players));
        }

        [HttpGet("{playerId}")]
        public ActionResult<PlayerDto> GetPlayer(Guid playerId)
        {
            var player = _playersRepository.GetPlayer(playerId);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PlayerDto>(player));
        }
//
//        [HttpOptions]
//        public IActionResult GetAuthorsOptions()
//        {
//            Response.Headers.Add("Allow", "GET,OPTIONS");
//            return Ok();
//        }
    }
}