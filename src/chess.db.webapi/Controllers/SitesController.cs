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
    [Route("api/sites")]
    public class SitesController : ResourceControllerBase<SiteDto, Site, Guid>
    {
        // NOTE: The ResourceControllerBase needs to know the names of the supported
        // routes. By default is assumes names based based on `nameof(TEntity)` that follow the pattern below
        // override with `ResourceControllerBase.HateoasConfig.XXXXRouteName.Set()` calls in the ctor or ctor
        // params if a different convention is required
        private const string GetSiteRouteName = "GetSite";
        private const string GetSitesRouteName = "GetSites";
        private const string CreateSiteRouteName = "CreateSite";
        private const string UpsertSiteRouteName = "UpsertSite";
        private const string PatchSiteRouteName = "PatchSite";
        private const string DeleteSiteRouteName = "DeleteSite";

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
            return ResourcesGet<object>(null, null, null);
        }

        [HttpGet("{id}", Name = GetSiteRouteName)]
        public IActionResult GetSite(Guid id)
            => ResourceGet(id);

        [HttpOptions]
        public IActionResult GetOptions()
            => ResourceOptions();
    }
}
