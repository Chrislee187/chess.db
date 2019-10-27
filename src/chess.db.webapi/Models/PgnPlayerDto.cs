using System;

namespace chess.db.webapi.Models
{
    public class PgnPlayerDto
    {
        public string Name { get; set; }
        public Guid? PlayerId { get; set; }
    }
}