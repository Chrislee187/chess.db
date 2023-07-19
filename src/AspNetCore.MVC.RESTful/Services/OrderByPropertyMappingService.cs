using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.RESTful.Services
{
    /// <summary>
    /// Service that maps <see cref="TDto"/> property names to one or
    /// more <see cref="TEntity"/> property names
    /// </summary>
    public class OrderByPropertyMappingService<TDto, TEntity> 
        : IOrderByPropertyMappingService<TDto, TEntity>
    {
        private readonly PropertyMapping _propertyMapping;

        public OrderByPropertyMappingService() 
            => _propertyMapping = new PropertyMapping(new Dictionary<string, OrderByPropertyMappingValue>());

        public OrderByPropertyMappingService(IDictionary<string, OrderByPropertyMappingValue> mappings) 
            => _propertyMapping = new PropertyMapping(mappings);

        public (bool Valid, ProblemDetails Details) ClauseIsValid(string fields)
        {
            var propertyMapping = _propertyMapping.MappingDictionary;

            if (string.IsNullOrWhiteSpace(fields))
            {
                return (true, null);
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    var propertyInfo = typeof(TDto).GetProperties().Any(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                    if (!propertyInfo)
                    {
                        return (false, new ProblemDetails()
                        {
                            Detail = $"orderby clause contains unknown/unmapped field: {propertyName}",
                            Title = "Invalid orderBy field/mapping"
                        });
                    }
                }
            }
            return (true, null);
        }

        public IDictionary<string, OrderByPropertyMappingValue> PropertyMapping() 
            => _propertyMapping.MappingDictionary;
    }
}