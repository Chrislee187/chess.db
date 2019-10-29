using System.ComponentModel.DataAnnotations;

namespace chess.db.webapi.Models
{
    public class PlayerCreationDto
    {
        public string Firstname { get; set; }
        public string MiddleNames { get; set; }
        
        [Required]
        public string Lastname { get; set; }
    }
}