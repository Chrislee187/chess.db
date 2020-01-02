using System;

namespace chess.games.db.Entities
{
    public class Event : DbEntity<Guid>, IHaveAName
    {
        public string Name { get; set; }
        public override string ToString() => Name;

    }

    public class PgnEvent : DbEntity<string>
    {
        public Event Event { get; set; }
    }
}