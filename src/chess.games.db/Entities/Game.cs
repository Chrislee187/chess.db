using System.ComponentModel.DataAnnotations;

namespace chess.games.db.Entities
{
    public class Game : DbEntity
    {
        public Event Event { get; set; }
        public Site Site { get; set; }
        public string PgnPlayerWhite { get; set; }
        public string PgnPlayerBlack { get; set; }

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
            return !string.IsNullOrEmpty(PgnPlayerBlack) && PgnPlayerBlack.Contains(name) 
                || !string.IsNullOrEmpty(PgnPlayerWhite) && PgnPlayerWhite.Contains(name);
        }
    }
}