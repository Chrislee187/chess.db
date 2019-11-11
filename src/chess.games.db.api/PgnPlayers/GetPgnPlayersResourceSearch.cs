using System.Linq;
using AspNetCore.MVC.RESTful.Parameters;
using chess.games.db.Entities;

namespace chess.games.db.api.PgnPlayers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GetPgnPlayersResourceSearch : IResourceSearch<PgnPlayer>
    {
        public IQueryable<PgnPlayer> Search(IQueryable<PgnPlayer> resources, string searchText)
            => resources.Where(p => p.Name.Contains(searchText));
    }
}