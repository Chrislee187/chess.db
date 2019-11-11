using System.Collections.Generic;
using AspNetCore.MVC.RESTful.Helpers;

namespace AspNetCore.MVC.RESTful.Services
{
    public class PropertyMapping : IPropertyMapping
    {
        public IDictionary<string, OrderByPropertyMappingValue> MappingDictionary { get; }

        public PropertyMapping(IDictionary<string, OrderByPropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = NullX.Throw(mappingDictionary,nameof(mappingDictionary));
        }
    }
}