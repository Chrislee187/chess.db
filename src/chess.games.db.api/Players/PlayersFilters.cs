using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PlayersFilters : Query<Player>
    {
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }

        public override bool Empty => new []{Firstname, Middlename, Lastname}.All(string.IsNullOrEmpty);

        public override IQueryable<Player> ApplyQuery(IQueryable<Player> set)
        {
            var result = set;

            if (!string.IsNullOrEmpty(Firstname))
                result = set.Where(p => p.Firstname.ToLower().Contains(Firstname.ToLower()));

            if (!string.IsNullOrEmpty(Middlename))
                result = set.Where(p => p.Middlenames.ToLower().Contains(Middlename.ToLower()));
            
            if (!string.IsNullOrEmpty(Lastname))
                result = set.Where(p => p.Surname.ToLower().Contains(Lastname.ToLower()));

            return result;
        }
    }
}