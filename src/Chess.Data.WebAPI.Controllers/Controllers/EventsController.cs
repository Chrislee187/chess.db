using Chess.Games.Data.Entities;
using EasyEF.Controllers;
using EasyEF.Repos;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Chess.Data.WebAPI.Controllers.Controllers;



public partial class EventsController : EasyEfController<EventEntity>
{

    public EventsController(ILinqRepository<EventEntity> repository) : base(repository)
    {

    }
}