using System;
using AspNetCore.MVC.RESTful.Controllers;

namespace chess.db.webapi.Models
{
    public class PgnPlayerDto : IResourceId

    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? PlayerId { get; set; }
    }
}