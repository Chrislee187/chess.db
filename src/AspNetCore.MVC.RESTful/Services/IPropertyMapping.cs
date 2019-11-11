using System.Collections.Generic;

namespace AspNetCore.MVC.RESTful.Services
{
    public interface IPropertyMapping
    {
        IDictionary<string, OrderByPropertyMappingValue> MappingDictionary { get; }
    }
}