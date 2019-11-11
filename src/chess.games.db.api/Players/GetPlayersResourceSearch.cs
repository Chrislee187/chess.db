using System.Linq;
using AspNetCore.MVC.RESTful.Parameters;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GetPlayersResourceSearch : IResourceSearch<Player>
    {
        public IQueryable<Player> Search(IQueryable<Player> resources, string searchText)
        {
            return resources.Where(p => p.Firstname.Contains(searchText)
                                    || p.Middlenames.Contains(searchText)
                                    || p.Surname.Contains(searchText));
        }
    }
}