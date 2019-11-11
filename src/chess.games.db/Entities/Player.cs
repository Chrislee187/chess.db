
using System;

namespace chess.games.db.Entities
{
    public class Player : DbEntity<Guid>
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Middlenames { get; set; }
    }
}