using System.Linq;
using AspNetCore.MVC.RESTful;
using AspNetCore.MVC.RESTful.Parameters;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GetPgnPlayersSearchQuery : Query<PgnPlayer>
    {
        public string QueryText { get; set; }

        public override bool Empty => string.IsNullOrEmpty(QueryText);

        public override IQueryable<PgnPlayer> ApplyQuery(IQueryable<PgnPlayer> set) 
            => set.Where(p => p.Name.Contains(QueryText));
    }
}