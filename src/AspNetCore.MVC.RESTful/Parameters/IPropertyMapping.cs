using System.Collections.Generic;

namespace AspNetCore.MVC.RESTful.Parameters
{
    public interface IPropertyMapping
    {
        IDictionary<string, OrderByPropertyMappingValue> MappingDictionary { get; }
    }
}