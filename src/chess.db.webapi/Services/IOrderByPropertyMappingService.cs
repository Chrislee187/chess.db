using System.Collections.Generic;
using chess.games.db.api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace chess.db.webapi.Services
{
    /// <summary>
    /// Maps Dto field names to one or more Entity field names for use when building
    /// order by clauses
    /// </summary>
    public interface IOrderByPropertyMappingService
    {
        Dictionary<string, OrderByPropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        
        (bool Valid, ProblemDetails Details) ClauseIsValid<TSource, TDestination>(string fields);
    }
}