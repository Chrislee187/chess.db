using System;

namespace chess.games.db.Entities
{
    public class Site : DbEntity<Guid>, IHaveAName
    {
        public string Name { get; set; }
    }
}