using System;
using System.Collections.Generic;
using chess.games.db.api.Helpers;

namespace chess.games.db.api
{
    public interface IResourceRepositoryBase<T> where T : class
    {
        void Add(T entity);
        
        bool Exists(Guid id);
        
        void Update(T player);
        
        T Get(Guid id);
        
        PagedList<T> Get(Query<T> filters = null,
            Query<T> query = null,
            PaginationParameters pagination = null,
            OrderByParameters orderByParameters = null,
            IDictionary<string, OrderByPropertyMappingValue> orderByMappings = null);
        
        void Delete(T entity);
        bool Save();
    }
}