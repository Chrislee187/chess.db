﻿using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Parameters;
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
    public class PlayerCollectionsController : ResourceControllerBase<PlayerDto, Player>
    {
        private readonly ILogger<PlayersController> _logger;
        private readonly IPlayersRepository _playersRepository;

        private const string GetPlayerCollectionRouteName = "GetPlayerCollection";

        public PlayerCollectionsController(
            IMapper mapper,
            IPlayersRepository playersRepository,
            IOrderByPropertyMappingService<PlayerDto, Player> orderByPropertyMappingService,
            IEntityUpdater<Player> entityUpdater,
            ILogger<PlayersController> logger
        ) : base(mapper, playersRepository, orderByPropertyMappingService, entityUpdater)
        {
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

            var entities = _playersRepository.Get(idList);

            if (idList.Count() != entities.Count())
            {
                return NotFound();
            }

            return Ok(Mapper.Map<IEnumerable<PlayerDto>>(entities));
        }

        [HttpPost]
        public ActionResult<IEnumerable<PlayerCreationDto>> CreatePlayerCollection(IEnumerable<PlayerCreationDto> playerCollection)
        {
            var entities = Mapper.Map<IEnumerable<Player>>(playerCollection);
            foreach (var player in entities)
            {
                _playersRepository.Add(player);
            }

            _playersRepository.Save();

            var added = Mapper.Map<IEnumerable<PlayerDto>>(entities);
            return CreatedAtRoute(
                GetPlayerCollectionRouteName,
                new {ids = string.Join(",", added.Select(a => a.Id)) },
                added);
        }
    }
}