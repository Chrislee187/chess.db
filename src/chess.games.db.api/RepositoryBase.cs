using System;
using System.Linq;

namespace chess.games.db.api
{
    public abstract class RepositoryBase
    {
        protected IQueryable<T> Reduce<T>(IQueryable<T> source, Query<T> filters, Query<T> query)
        {
            if (filters.Empty && query.Empty)
            {
                return source;
            }

            var set = source;

            if (!filters.Empty)
            {
                set = filters.Apply(set);
            }

            if (!query.Empty)
            {
                set = query.Apply(set);
            }

            return set;
        }
    }
}