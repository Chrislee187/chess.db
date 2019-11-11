using System;
using AspNetCore.MVC.RESTful.Controllers;

namespace chess.db.webapi.Models
{
    public class PlayerDto : IResourceId<Guid>
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string MiddleNames { get; set; }
        public string Lastname { get; set; }
    }
}