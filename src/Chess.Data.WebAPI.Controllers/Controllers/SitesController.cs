using Chess.Games.Data.Entities;
using EasyEF.Controllers;
using EasyEF.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Data.WebAPI.Controllers.Controllers;

public class SitesController : EasyEfController<SiteEntity>
{

    public SitesController(ILinqRepository<SiteEntity> repository) : base(repository)
    {

    }

}