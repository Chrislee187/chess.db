using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PgnPlayersSearchQuery : Query<PgnPlayer>
    {
        public string QueryText { get; set; }

        public override bool Empty => string.IsNullOrEmpty(QueryText);

        public override IQueryable<PgnPlayer> ApplyQuery(IQueryable<PgnPlayer> set) 
            => set.Where(p => p.Name.Contains(QueryText));
    }
}