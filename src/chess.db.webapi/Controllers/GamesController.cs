using System;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;
using AutoMapper;
using chess.db.webapi.Models;
using chess.games.db.api.Repositories;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Mvc;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ResourceControllerBase<GameDto, Game, Guid>
    {
        private const string GetGameRouteName = "GetGame";
        private const string GetGamesRouteName = "GetGames";

        public GamesController(IMapper mapper,
            IGamesRepository gamesRepository,
            IOrderByPropertyMappingService<GameDto, Game> orderByPropertyMappingService,
            IEntityUpdater<Game, Guid> entityUpdater)
            : base(mapper, gamesRepository, entityUpdater, orderByPropertyMappingService)
        {
        }

        [HttpGet(Name = GetGamesRouteName)]
        [HttpHead]
        public IActionResult GetGames()
        {
            return ResourcesGet();
        }

        [HttpGet("{id}", Name = GetGameRouteName)]
        public IActionResult GetGame(Guid id)
            => ResourceGet(id);
    }
}
