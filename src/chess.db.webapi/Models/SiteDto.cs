using System;
using AspNetCore.MVC.RESTful.Controllers;

namespace chess.db.webapi.Models
{
    public class SiteDto : Resource<Guid>
    {
        public string Name { get; set; }
    }
}