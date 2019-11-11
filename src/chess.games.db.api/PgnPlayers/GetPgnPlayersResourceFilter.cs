using System.Linq;
using AspNetCore.MVC.RESTful.Parameters;
using chess.games.db.Entities;

namespace chess.games.db.api.PgnPlayers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GetPgnPlayersResourceFilter : ResourceFilter<PgnPlayer>
    {
        public string Name { get; set; }

        public override bool Empty => string.IsNullOrEmpty(Name);

        protected override IQueryable<PgnPlayer> FilterImpl(IQueryable<PgnPlayer> resources) 
            => resources.Where(p => p.Name.ToLower().Contains(Name.ToLower()));
    }
}