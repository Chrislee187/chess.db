using Chess.Games.Data.Entities;
using EasyEF.Controllers;
using EasyEF.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Data.WebAPI.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : EasyEfController<PlayerEntity>
{

    public PlayersController(ILinqRepository<PlayerEntity> repository) : base(repository)
    {

    }

}