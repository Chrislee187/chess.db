using System.Linq;
using AspNetCore.MVC.RESTful.Parameters;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class GetPlayersSearchQuery : Query<Player>
    {
        public string QueryText { get; set; }

        public override bool Empty => string.IsNullOrEmpty(QueryText);

        public override IQueryable<Player> ApplyQuery(IQueryable<Player> set) 
            => set.Where(p => p.Firstname.Contains(QueryText) 
                              || p.Middlenames.Contains(QueryText) 
                              || p.Surname.Contains(QueryText));
    }
}