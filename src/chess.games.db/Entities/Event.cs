using System;

namespace chess.games.db.Entities
{
    public class Event : DbEntity<Guid>, IHaveAName
    {
        public string Name { get; set; }
    }
}