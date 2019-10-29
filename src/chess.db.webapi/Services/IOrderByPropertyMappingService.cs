using System.Collections.Generic;
using chess.games.db.api.Helpers;

namespace chess.db.webapi.Services
{
    public interface IOrderByPropertyMappingService
    {
        Dictionary<string, OrderByPropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool ClauseIsValid<TSource, TDestination>(string fields);
    }
}