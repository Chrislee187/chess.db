using System;
using System.ComponentModel.DataAnnotations;

namespace chess.db.webapi.Models
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string MiddleNames { get; set; }
        public string Lastname { get; set; }
    }

    public class PlayerCreationDto
    {
        public string Firstname { get; set; }
        public string MiddleNames { get; set; }
        
        [Required]
        public string Lastname { get; set; }
    }
}