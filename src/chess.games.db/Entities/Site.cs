using System;

namespace chess.games.db.Entities
{
    public class Site : DbEntity<Guid>, IHaveAName
    {
        public string Name { get; set; }

        public override string ToString() => Name;
    }


    public class PgnSite : DbEntity<string>
    {
        public Site Site { get; set; }
    }
}