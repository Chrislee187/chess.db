using System;
using AspNetCore.MVC.RESTful.Controllers;

namespace chess.db.webapi.Models
{
    public class GameDto : Resource<Guid>
    {
        public Guid EventId { get; set; }
        public string Event { get; set; }
        public Guid SiteId { get; set; }
        public string Site { get; set; }
        public Guid WhiteId { get; set; }
        public string White { get; set; }
        public Guid BlackId { get; set; }
        public string Black { get; set; }
        public string Date { get; set; }
        public string Round { get; set; }
        public string Result { get; set; }
        public string Moves { get; set; }
    }
}