using System;
using System.ComponentModel.DataAnnotations;

namespace chess.games.db.Entities
{
    public class PgnGame : DbEntity<Guid>
    {
        // The mandatory seven PGN tags
        [Required][MaxLength(450)]
        public string Event { get; set; }
        [Required]
        [MaxLength(450)]
        public string Site { get; set; }
        [Required]
        [MaxLength(450)]
        public string White { get; set; }
        [Required]
        [MaxLength(450)]
        public string Black { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string Round { get; set; }
        [Required]
        public string Result { get; set; }

        [Required]
        public string MoveList { get; set; }
        
        // Very commonly used tags
        public string Eco { get; set; }
        public string WhiteElo { get; set; }
        public string BlackElo { get; set; }

        // Any additional tags are stored as Json
        public string CustomTagsJson { get; set; }
    }
}