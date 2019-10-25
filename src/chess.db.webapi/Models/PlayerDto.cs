using System;

namespace chess.db.webapi.Models
{
    public class PlayerDto
    {
        public string Name { get; set; }
        public Guid Id
        { get; set; }
    }
}