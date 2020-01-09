using System;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;
using AutoMapper;
using chess.db.webapi.Models;
using chess.games.db.api.Repositories;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace chess.db.webapi.Controllers
{
    [ApiController]
    [Route("api/sites")]
    public class SitesController : ResourceControllerBase<SiteDto, Site, Guid>
    {
        private const string SiteRouteName = "GetSite";
        private const string SitesRouteName = "GetSites";

        public SitesController(IMapper mapper,
            ISitesRepository gamesRepository,
            IOrderByPropertyMappingService<SiteDto, Site> orderByPropertyMappingService,
            IEntityUpdater<Site, Guid> entityUpdater,
            ILogger<SitesController> logger)
            : base(mapper, gamesRepository, entityUpdater, orderByPropertyMappingService, logger: logger)
        {
        }

        [HttpGet(Name = SitesRouteName)]
        [HttpHead]
        public IActionResult Sites()
        {
            return ResourcesGet();
        }

        [HttpGet("{id}", Name = SiteRouteName)]
        public IActionResult Site(Guid id)
            => ResourceGet(id);
    }
}
