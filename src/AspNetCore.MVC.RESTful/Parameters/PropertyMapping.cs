using System;
using System.Collections.Generic;

namespace AspNetCore.MVC.RESTful.Parameters
{
    public class PropertyMapping : IPropertyMapping
    {
        public IDictionary<string, OrderByPropertyMappingValue> MappingDictionary { get; }

        public PropertyMapping(IDictionary<string, OrderByPropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary ??
                                throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}