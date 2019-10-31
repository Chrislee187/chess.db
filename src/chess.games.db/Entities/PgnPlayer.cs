using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AspNetCore.MVC.RESTful.Controllers;

namespace chess.games.db.Entities
{
    public class PgnPlayer : IResourceEntity, IHaveAName
    {
        [Key]
        public string Name { get; set; }

        public Player Player { get; set; }

        [NotMapped]
        public Guid Id { get; set; }
    }
}