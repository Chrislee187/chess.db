using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class GetPlayersEntityFilter : EntityFilter<Player>
    {
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }

        public override bool Empty => new []{Firstname, Middlename, Lastname}.All(string.IsNullOrEmpty);

        protected override IQueryable<Player> FilterImpl(IQueryable<Player> resources)
        {
            var result = resources;

            if (!string.IsNullOrEmpty(Firstname))
                result = resources.Where(p => p.Firstname.ToLower().Contains(Firstname.ToLower()));

            if (!string.IsNullOrEmpty(Middlename))
                result = resources.Where(p => p.Middlenames.ToLower().Contains(Middlename.ToLower()));
            
            if (!string.IsNullOrEmpty(Lastname))
                result = resources.Where(p => p.Surname.ToLower().Contains(Lastname.ToLower()));

            return result;
        }
    }
}