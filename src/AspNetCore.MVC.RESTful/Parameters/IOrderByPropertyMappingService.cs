using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.RESTful.Parameters
{
    /// <summary>
    /// Maps Dto field names to one or more Entity field names for use when building
    /// order by clauses
    /// </summary>
    public interface IOrderByPropertyMappingService<TDto, TEntity>
    {
        IDictionary<string, OrderByPropertyMappingValue> GetPropertyMapping();
        
        (bool Valid, ProblemDetails Details) ClauseIsValid(string fields);
    }
}