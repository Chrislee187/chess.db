using System;
using System.Diagnostics.CodeAnalysis;

namespace chess.games.db.Entities
{
    public class PgnPlayer : DbEntity, IHaveAName
    {
        public string Name { get; set; }

        public Player Player { get; set; }
    }
}