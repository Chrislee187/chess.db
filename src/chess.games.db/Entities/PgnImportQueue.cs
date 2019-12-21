using System;
using System.ComponentModel.DataAnnotations;

namespace chess.games.db.Entities
{
    public class PgnImportQueue : DbEntity<Guid>
    {
        // The mandatory seven PGN tags
        [Required]
        public string Event { get; set; }
        [Required]
        public string Site { get; set; }
        [Required]
        public string White { get; set; }
        [Required]
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

        public bool ImportNormalisationComplete { get; set; } = false;
    }


}