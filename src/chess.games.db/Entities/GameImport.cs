using System;
using System.ComponentModel.DataAnnotations;

namespace chess.games.db.Entities
{
    public class GameImport : DbEntity<Guid>
    {
        public Event Event { get; set; }
        public Site Site { get; set; }
        public PgnPlayer White { get; set; }
        public PgnPlayer Black { get; set; }

        [MaxLength(30)]
        public string Date { get; set; }

        public string Round { get; set; }
        public GameResult Result { get; set; }

        public string MoveText { get; set; }

        public string Eco { get; set; }
    }
}