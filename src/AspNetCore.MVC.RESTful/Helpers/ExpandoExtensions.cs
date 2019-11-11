using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public static class ExpandoExtensions
    {
        public static ExpandoObject ToExpando(this object obj)
        {
            var expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;

            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                dictionary.Add(property.Name, property.GetValue(obj));

            return expando;
        }

        public static ExpandoObject ToExpandObject(this IEnumerable<KeyValuePair<string, object>> dictionary)
            => ToExpandObject(dictionary.ToDictionary(k => k.Key, v => v.Value));
        
        public static ExpandoObject ToExpandObject(this IDictionary<string, object> dictionary)
        {
            var result = new ExpandoObject();
            var resultDict = (IDictionary<string, object>) result;
            foreach (var (key, value) in dictionary)
            {
                resultDict.Add(key, value);
            }

            return result;
        }
    }
}