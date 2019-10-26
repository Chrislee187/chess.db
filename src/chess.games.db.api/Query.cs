using System.Linq;

namespace chess.games.db.api
{
    public abstract class Query<T>
    {
        public abstract bool Empty { get; }
        public abstract IQueryable<T> ApplyQuery(IQueryable<T> set);

        public IQueryable<T> Apply(IQueryable<T> set)
        {
            if (!Empty)
            {
                set = ApplyQuery(set);
            }

            return set;
        }
    }
}