using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public static class ShapeDataExtension
    {
        private const BindingFlags ShapeablePropertyBindingFlags = BindingFlags.IgnoreCase
                                                                  | BindingFlags.Public 
                                                                  | BindingFlags.Instance;

        /// <summary>
        /// Reshape objects to contain only the fields specified in the <paramref name="shape">shape</paramref>.
        /// <para>
        /// Because an ExpandoObject is treated as dictionary during serialization, 
        /// default serializer NullValue handling will not be 
        /// used as this functionality is for properties not dictionary
        /// values.
        /// </para>
        /// <see href="https://github.com/JamesNK/Newtonsoft.Json/issues/951">Newtonsoft Issue 951</see>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="shape">Comma separated list of property names to placed in the expando object</param>
        /// <returns>List of ExpandoObjects with only the properties specified by <paramref name="shape"/> on them</returns>
        public static IEnumerable<ExpandoObject> ShapeData<T>(
            [NotNull] this IEnumerable<T> source,
            string shape)
        {
            var expandoObjectList = new List<ExpandoObject>();

            var propertyNames = (shape ?? "").Split(',')
                .Where(s => !string.IsNullOrEmpty(s.Trim())).ToList();

            var shapeProperties = Properties<T>(propertyNames.ToArray()).ToList();

            foreach (var sourceObject in source)
            {
                var expandoObject = shapeProperties
                    .ToDictionary(k => k.Name, v => v.GetValue(sourceObject))
                    .ToExpando();

                expandoObjectList.Add(expandoObject);
            }

            return expandoObjectList;
        }

        public static ExpandoObject ShapeData<T>(this T source,
            string shape)
        {
            if (string.IsNullOrWhiteSpace(shape)) return source.ToExpando();

            string[] propertyNames = shape.Split(',')
                .Where(s => !string.IsNullOrEmpty(s.Trim())).ToArray();


            var expando = Properties<T>(propertyNames).ToList()
                .ToDictionary(k => k.Name, v => v.GetValue(source))
                .ToExpando();

            return expando;
        }

        private static IEnumerable<PropertyInfo> Properties<T>(string[] shape)
        {
            var infos = new List<PropertyInfo>();

            if (!shape.Any())
            {
                infos.AddRange(typeof(T)
                    .GetProperties(ShapeablePropertyBindingFlags)
                    .Where(info => !HasSerializationIgnoreAttribute(info))
                );
            }
            else
            {
                foreach (var propertyName in shape.Select(f => f.Trim()))
                {
                    var info = typeof(T).GetProperty(propertyName, ShapeablePropertyBindingFlags);

                    if (info == null)
                    {
                        throw new ArgumentException($"Property '{propertyName}' from the shape doesn't exist on the type: '{typeof(T)}'");
                    }
                    
                    if (!info.HasSerializationIgnoreAttribute())
                    {
                        infos.Add(info);
                    }
                }
            }

            return infos;
        }

        public static bool HasSerializationIgnoreAttribute(this PropertyInfo info) 
            => info.CustomAttributes
                .Any(a => new[] {"JsonIgnore", "XmlIgnore" }
                    .Any(ignoreAttr => a.AttributeType.Name.Contains(ignoreAttr, StringComparison.InvariantCulture)));
    }

}