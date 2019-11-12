using System;
using AspNetCore.MVC.RESTful.Controllers;

namespace chess.db.webapi.Models
{
    public class PlayerDto : Resource<Guid>
    {
        public string Firstname { get; set; }
        public string MiddleNames { get; set; }
        public string Lastname { get; set; }
    }
}