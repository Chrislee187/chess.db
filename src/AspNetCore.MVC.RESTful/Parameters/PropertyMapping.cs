using System;
using System.Collections.Generic;

namespace AspNetCore.MVC.RESTful.Parameters
{
    // ReSharper disable once UnusedTypeParameter
    // NOTE: Generic types here are used as keys to differentiate between mappings
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public IDictionary<string, OrderByPropertyMappingValue> MappingDictionary { get; private set; }

        public PropertyMapping(IDictionary<string, OrderByPropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary ??
                                throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}