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
    /// <summary>
    /// PgnPlayers are created by the import mechanism and are not creatable or deletable by the API.
    /// They represent the original data from the original source before attempting to dedupe the data
    /// </summary>
    [ApiController]
    [Route("api/pgnplayers")]
    public class PgnPlayersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PgnPlayersController> _logger;
        private readonly IPgnPlayersRepository _pgnPlayersRepository;

        public PgnPlayersController(
            IMapper mapper,
            IPgnPlayersRepository pgnPlayersRepository,
            ILogger<PgnPlayersController> logger
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _pgnPlayersRepository = pgnPlayersRepository ?? throw new ArgumentNullException(nameof(pgnPlayersRepository));
            _logger = logger ?? NullLogger<PgnPlayersController>.Instance;
        }


        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<PgnPlayerDto>> GetPgnPlayers(
            [FromQuery] PgnPlayerResourceParameters parameters
            )
        {
            var filters = _mapper.Map<PgnPlayersFilters>(parameters);
            var query = _mapper.Map<PgnPlayersSearchQuery>(parameters);

            var players = _pgnPlayersRepository
                .GetPgnPlayers(filters, query)
                .Take(1000); // TODO: Temp restriction until paging is implemented

            return Ok(_mapper.Map<IEnumerable<PgnPlayerDto>>(players));
        }

        [HttpGet("{name}")]
        public ActionResult<PgnPlayerDto> GetPgnPlayer(string name)
        {
            var player = _pgnPlayersRepository.GetPgnPlayer(name);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PgnPlayerDto>(player));
        }

        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
    }
}