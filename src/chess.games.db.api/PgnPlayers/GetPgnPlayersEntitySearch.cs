using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using chess.games.db.Entities;

namespace chess.games.db.api.PgnPlayers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GetPgnPlayersEntitySearch : IEntitySearch<PgnPlayer>
    {
        public IQueryable<PgnPlayer> Search(IQueryable<PgnPlayer> entities, string searchText)
            => entities.Where(p => p.Player.LastName.Contains(searchText));
    }
}