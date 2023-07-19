using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.RESTful.Services
{
    /// <summary>
    /// Maps Dto field names to one or more Entity field names for use when building
    /// order by clauses
    /// </summary>
    // ReSharper disable UnusedTypeParameter
    public interface IOrderByPropertyMappingService<TDto, TEntity>
        // ReSharper restore UnusedTypeParameter
    {
        IDictionary<string, OrderByPropertyMappingValue> PropertyMapping();
        
        (bool Valid, ProblemDetails Details) ClauseIsValid(string fields);
    }
}