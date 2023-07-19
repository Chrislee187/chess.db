using Chess.Games.Data.Entities;
using EasyEF.Controllers;
using EasyEF.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Data.WebAPI.Controllers.Controllers;

public class GamesController : EasyEfController<GameEntity>
{
    public GamesController(ILinqRepository<GameEntity> repository) : base(repository)
    {

    }
}