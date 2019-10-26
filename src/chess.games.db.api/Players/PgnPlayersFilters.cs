using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PgnPlayersFilters : Query<PgnPlayer>
    {
        public string Name { get; set; }

        public override bool Empty => string.IsNullOrEmpty(Name);

        public override IQueryable<PgnPlayer> ApplyQuery(IQueryable<PgnPlayer> set) 
            => set.Where(p => p.Name.ToLower().Contains(Name.ToLower()));
    }
}