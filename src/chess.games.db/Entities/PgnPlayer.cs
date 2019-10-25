using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace chess.games.db.Entities
{
    public class PgnPlayer : IHaveAName
    {
        [Key]
        public string Name { get; set; }

        public Player Player { get; set; }
    }
}