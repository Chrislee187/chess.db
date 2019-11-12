using System;
using System.Xml.Serialization;
using AspNetCore.MVC.RESTful.Controllers;
using Newtonsoft.Json;

namespace chess.db.webapi.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PgnPlayerDto : Resource<Guid>
    {
        [JsonIgnore][XmlIgnore]
        public new Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? PlayerId { get; set; }
    }
}