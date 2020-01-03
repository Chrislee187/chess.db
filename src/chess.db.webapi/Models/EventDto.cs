using System;
using AspNetCore.MVC.RESTful.Controllers;

namespace chess.db.webapi.Models
{
    public class EventDto : Resource<Guid>
    {
        public string Name { get; set; }
    }
}