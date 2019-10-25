using System;

namespace chess.db.webapi.Models
{
    public class PgnPlayerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}