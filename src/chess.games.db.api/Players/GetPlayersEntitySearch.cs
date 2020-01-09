using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayersEntitySearch : IEntitySearch<Player>
    {
        public IQueryable<Player> Search(IQueryable<Player> entities, string searchText)
        {
            return entities.Where(p => p.Firstname.Contains(searchText)
                                    || p.OtherNames.Contains(searchText)
                                    || p.LastName.Contains(searchText));
        }
    }
}