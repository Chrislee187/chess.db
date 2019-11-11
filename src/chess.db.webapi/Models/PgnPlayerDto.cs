using System;
using System.Xml.Serialization;
using AspNetCore.MVC.RESTful.Controllers;
using Newtonsoft.Json;

namespace chess.db.webapi.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PgnPlayerDto : IResourceId<Guid>
    {
        [JsonIgnore][XmlIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? PlayerId { get; set; }
    }
}