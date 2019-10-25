using System;
using System.Linq;
using chess.games.db.api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly ILogger<PlayersController> _logger;
        private readonly IPlayersRepository _playersRepository;

        public PlayersController(
            IPlayersRepository playersRepository,

            ILogger<PlayersController> logger
            )
        {
            _playersRepository = playersRepository ?? throw new ArgumentNullException(nameof(playersRepository));
            _logger = logger;
        }

        [HttpGet()]
        [HttpHead]
        public IActionResult GetPlayers()
        {
            var players = _playersRepository.GetPlayers().Take(1000); // TODO: Temp restriction until paging is implemented
            return Ok(players);
        }

        [HttpGet("{playerId}")]
        public IActionResult GetPlayer(Guid playerId)
        {
            var player = _playersRepository.GetPlayer(playerId);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(player);
        }

    }
}