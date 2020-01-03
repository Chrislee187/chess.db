using System;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;
using AutoMapper;
using chess.db.webapi.Models;
using chess.games.db.api.Players;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Mvc;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ResourceControllerBase<GameDto, Game, Guid>
    {
        // NOTE: The ResourceControllerBase needs to know the names of the supported
        // routes. By default is assumes names based based on `nameof(TEntity)` that follow the pattern below
        // override with `ResourceControllerBase.HateoasConfig.XXXXRouteName.Set()` calls in the ctor or ctor
        // params if a different convention is required
        private const string GetGameRouteName = "GetGame";
        private const string GetGamesRouteName = "GetGames";
        private const string CreateGameRouteName = "CreateGame";
        private const string UpsertGameRouteName = "UpsertGame";
        private const string PatchGameRouteName = "PatchGame";
        private const string DeleteGameRouteName = "DeleteGame";

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
            return ResourcesGet<object>(null, null, null);
        }

        [HttpGet("{id}", Name = GetGameRouteName)]
        public IActionResult GetGame(Guid id)
            => ResourceGet(id);

        [HttpOptions]
        public IActionResult GetOptions()
            => ResourceOptions();
    }
}
