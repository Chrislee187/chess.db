using System.ComponentModel.DataAnnotations;

namespace chess.games.db.Entities
{
    public class PgnPlayer : IHaveAName
    {
        [Key]
        public string Name { get; set; }

        public Player Player { get; set; }
    }
}