using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using chess.db.webapi.Models;
using chess.games.db.api.Players;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/playercollections")]
    public class PlayerCollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PlayersController> _logger;
        private readonly IPlayersRepository _playersRepository;

        private const string GetPlayerCollectionRouteName = "GetPlayerCollection";

        public PlayerCollectionsController(
            IMapper mapper,
            IPlayersRepository playersRepository,
            ILogger<PlayersController> logger
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _playersRepository = playersRepository ?? throw new ArgumentNullException(nameof(playersRepository));
            _logger = logger ?? NullLogger<PlayersController>.Instance;
        }

        [HttpGet("({ids})", Name = GetPlayerCollectionRouteName)]
        public IActionResult GetPlayerCollection([FromRoute] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var idList = ids as Guid[] ?? ids.ToArray();
            if (!idList.Any())
            {
                return Ok(new List<PlayerDto>());
            }

            var entities = _playersRepository.GetPlayers(idList);

            if (idList.Count() != entities.Count())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<PlayerDto>>(entities));
        }

        [HttpPost]
        public ActionResult<IEnumerable<PlayerDto>> CreatePlayerCollection(IEnumerable<PlayerCreationDto> playerCollection)
        {
            var entities = _mapper.Map<IEnumerable<Player>>(playerCollection);
            foreach (var player in entities)
            {
                _playersRepository.Add(player);
            }

            _playersRepository.Save();

            var added = _mapper.Map<IEnumerable<PlayerDto>>(entities);
            return CreatedAtRoute(
                GetPlayerCollectionRouteName,
                new {ids = string.Join(",", added.Select(a => a.Id)) },
                added);
        }
    }
}