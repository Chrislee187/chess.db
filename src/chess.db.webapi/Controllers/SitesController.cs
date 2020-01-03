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
    [Route("api/sites")]
    public class SitesController : ResourceControllerBase<SiteDto, Site, Guid>
    {
        private const string GetSiteRouteName = "GetSite";
        private const string GetSitesRouteName = "GetSites";

        public SitesController(IMapper mapper,
            ISitesRepository gamesRepository,
            IOrderByPropertyMappingService<SiteDto, Site> orderByPropertyMappingService,
            IEntityUpdater<Site, Guid> entityUpdater)
            : base(mapper, gamesRepository, entityUpdater, orderByPropertyMappingService)
        {
        }

        [HttpGet(Name = GetSitesRouteName)]
        [HttpHead]
        public IActionResult GetSites()
        {
            return ResourcesGet();
        }

        [HttpGet("{id}", Name = GetSiteRouteName)]
        public IActionResult GetSite(Guid id)
            => ResourceGet(id);
    }
}
