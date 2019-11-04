using System.Linq;
using AspNetCore.MVC.RESTful.Parameters;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GetPlayersSearchQuery : ResourceQuery<Player>
    {
        public string QueryText { get; set; }

        public override bool Empty => string.IsNullOrEmpty(QueryText);

        protected override IQueryable<Player> ApplyQuery(IQueryable<Player> resources) 
            => resources.Where(p => p.Firstname.Contains(QueryText) 
                              || p.Middlenames.Contains(QueryText) 
                              || p.Surname.Contains(QueryText));
    }
}