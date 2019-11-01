using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace chess.games.db.Entities
{
    public class PgnPlayer : IHaveAName, IDbEntity
    {
        [Key]
        public string Name { get; set; }

        public Player Player { get; set; }

        [NotMapped]
        public Guid Id { get; set; }
    }
}