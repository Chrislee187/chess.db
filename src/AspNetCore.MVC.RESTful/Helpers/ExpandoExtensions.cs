using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.MVC.RESTful.Helpers
{
    /// <summary>
    /// Extension methods to assist with using <see cref="ExpandoObject"/> objects.
    /// </summary>
    public static class ExpandoExtensions
    {
        /// <summary>
        /// Converts any <see cref="object"/> to an <see cref="ExpandoObject"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ExpandoObject ToExpando(this object obj)
        {
            var expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;

            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                dictionary.Add(property.Name, property.GetValue(obj));

            return expando;
        }

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>
        /// of <see cref="string"/> and <see cref="object"/> to an <see cref="ExpandoObject"/>
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static ExpandoObject ToExpando(this IEnumerable<KeyValuePair<string, object>> dictionary)
            => ToExpando(dictionary.ToDictionary(k => k.Key, v => v.Value));

        /// <summary>
        /// Converts an <see cref="IDictionary{TKey,TValue}"/>
        /// of <see cref="string"/> and <see cref="object"/> to an <see cref="ExpandoObject"/>
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var result = new ExpandoObject();
            var resultDict = (IDictionary<string, object>) result;
            foreach (var (key, value) in dictionary)
            {
                resultDict.Add(key, value);
            }

            return result;
        }

        /// <summary>
        /// Converts any <see cref="object"/> to a <see cref="IDictionary{TKey,TValue}"/>
        /// of <see cref="string"/> and <see cref="object"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToExpandoDict(this object obj) => obj.ToExpando();

        /// <summary>
        /// Converts an <see cref="ExpandoObject"/> to a <see cref="IDictionary{TKey,TValue}"/>
        /// of <see cref="string"/> and <see cref="object"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToExpandoDict(this ExpandoObject obj) => obj;

        public static T ExtractProperty<T>(this ExpandoObject obj, string propertyName)
        {
            var resourcesDict = (IDictionary<string, object>)obj;

            if (resourcesDict.TryGetValue(propertyName, out var linksObject))
            {
                return (T)linksObject;
            }

            return default;
        }
    }
}