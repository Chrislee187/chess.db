using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public static class IDictionaryExtensions
    {
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