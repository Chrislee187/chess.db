using System;
using System.ComponentModel.DataAnnotations;

namespace chess.games.db.Entities
{
    public class PgnPlayer : IHaveAName, IDbEntity
    {
        [Key]
        public string Name { get; set; }

        public Player Player { get; set; }
        public Guid Id { get; set; }
    }
}