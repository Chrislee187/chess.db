using System.Linq;
using AspNetCore.MVC.RESTful.Parameters;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GetPgnPlayersFilters : ResourceQuery<PgnPlayer>
    {
        public string Name { get; set; }

        public override bool Empty => string.IsNullOrEmpty(Name);

        public override IQueryable<PgnPlayer> ApplyQuery(IQueryable<PgnPlayer> resources) 
            => resources.Where(p => p.Name.ToLower().Contains(Name.ToLower()));
    }
}