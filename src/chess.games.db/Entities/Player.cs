using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace chess.games.db.Entities
{
    public class Player : DbEntity<Guid>
    {
        public string Firstname { get; set; }
        
        [Required]
        public string LastName { get; set; }

        public string OtherNames { get; set; }

        public override string ToString()
        {
            return $"{Firstname} {OtherNames} {LastName}";
        }

        public Player()
        {
            
        }

        public Player(string firstname, string otherNames, string lastname)
        {
            Firstname = firstname;
            OtherNames = otherNames;
            LastName = lastname;
        }
    }
    public class PgnPlayer : DbEntity<string>
    {
        public Player Player { get; set; }
    }
}