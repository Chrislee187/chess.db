using System.Linq;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace chess.games.db.api
{
    public abstract class Query<T>
    {
        public static readonly Query<T> Default = new EmptyQuery();
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

        private class EmptyQuery : Query<T>
        {
            public override bool Empty { get; } = true;
            public override IQueryable<T> ApplyQuery(IQueryable<T> set) => set;
        }    
    }

    
}