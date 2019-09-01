using System;
using System.ComponentModel.DataAnnotations;

namespace chess.games.db.Entities
{
    public class Game : DbEntity
    {
        public Event Event { get; set; }
        public Site Site { get; set; }
        public Player White { get; set; }
        public Player Black { get; set; }

        [MaxLength(30)]
        public string Date { get; set; }

        public string Round { get; set; }
        public GameResult Result { get; set; }

        public string MoveText { get; set; }

        /// <summary>
        /// Chess Opening code
        /// <see cref="https://en.wikipedia.org/wiki/Encyclopaedia_of_Chess_Openings"/>
        /// </summary>
        public string ECO { get; set; }

        public bool ContainsPlayer(string name)
        {
            if (Black == null || White == null) return false;
            return Black.Name.Contains(name) || White.Name.Contains(name);
        }
    }
}