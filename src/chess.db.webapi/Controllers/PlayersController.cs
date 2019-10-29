﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AutoMapper;
using chess.db.webapi.Helpers;
using chess.db.webapi.Models;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api;
using chess.games.db.api.Players;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PlayersController> _logger;
        private readonly IPlayersRepository _playersRepository;

        private const string GetPlayerRouteName = "Get";
        private const string GetPlayersRouteName = "GetPlayers";
        private const string UpsertPlayerRouteName = "UpsertPlayer";

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

        [HttpGet(Name = GetPlayersRouteName)]
        [HttpHead]
        public ActionResult<IEnumerable<PlayerDto>> GetPlayers(
            [FromQuery] PlayerResourceParameters parameters
        )
        {
            var filters = _mapper.Map<PlayersFilters>(parameters);
            var query = _mapper.Map<PlayersSearchQuery>(parameters);
            var pages = _mapper.Map<PaginationParameters>(parameters);

            var players = _playersRepository
                .Get(filters, query, pages);

            var urls = CreatePrevNextUris(players, filters, query, pages);
            AddPaginationHeader(players, urls.previous, urls.next);

            return Ok(_mapper.Map<IEnumerable<PlayerDto>>(players));
        }


        [HttpGet("{id}", Name = GetPlayerRouteName)]
        public ActionResult<PlayerDto> GetPlayer(Guid id)
        {
            var player = _playersRepository.Get(id);

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
                createdPlayer
                );
        }

        [HttpPut("{id}", Name = UpsertPlayerRouteName)]
        public ActionResult UpsertPlayer(Guid id, PlayerUpdateDto model)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }

            var player = _playersRepository.Get(id);
            
            ActionResult result;
            if (player == null)
            {
                var addedPlayer = _mapper.Map<Player>(model);
                addedPlayer.Id = id;
                _playersRepository.Add(addedPlayer);

                var playerResult = _mapper.Map<PlayerDto>(addedPlayer);

                result = CreatedAtRoute(
                    GetPlayerRouteName,
                    new { id },
                    playerResult
                    );
            }
            else
            {
                _mapper.Map(model, player);
                _playersRepository.Update(player);
                result = NoContent();
            }
            _playersRepository.Save();

            return result;
        }

        [HttpPatch("{id}")]
        public ActionResult PatchPlayer(Guid id,
            JsonPatchDocument<PlayerUpdateDto> patchDocument)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }

            var player = _playersRepository.Get(id);
            if (player == null)
            {
                return NotFound();
            }

            var patchedPlayer = _mapper.Map<PlayerUpdateDto>(player);
            patchDocument.ApplyTo(patchedPlayer, ModelState);

            if (!TryValidateModel(patchedPlayer))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(patchedPlayer, player);
            _playersRepository.Update(player);
            _playersRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePlayer(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }

            var player = _playersRepository.Get(id);

            if (player == null)
            {
                return NotFound();
            }

            _playersRepository.Delete(player);
            _playersRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "HEAD,OPTIONS,GET,POST,PUT,PATCH,DELETE");
            return Ok();
        }

        private (string previous, string next) CreatePrevNextUris(PagedList<Player> players, PlayersFilters filters, PlayersSearchQuery query, PaginationParameters pages)
        {
            var previous = players.HasPrevious
                ? CreatePlayersResourceUri(filters, query, pages, ResourceUriType.PreviousPage)
                : null;
            var next = players.HasNext
                ? CreatePlayersResourceUri(filters, query, pages, ResourceUriType.NextPage)
                : null;

            return (previous, next);
        }

        private string CreatePlayersResourceUri(
                        PlayersFilters filter,
                        PlayersSearchQuery query,
                        PaginationParameters pages,
                        ResourceUriType type)
        {

            var uriParams = new PlayerResourceParameters
            {
                PageSize = pages.PageSize,
                FirstnameFilter = filter.Firstname,
                MiddlenameFilter = filter.Middlename,
                LastnameFilter = filter.Lastname,
                SearchQuery = query.QueryText
            };
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    uriParams.PageNumber -= 1;
                    break;
                case ResourceUriType.NextPage:
                    uriParams.PageNumber += 1;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return Url.Link(GetPlayersRouteName,uriParams);

        }
    }
}
