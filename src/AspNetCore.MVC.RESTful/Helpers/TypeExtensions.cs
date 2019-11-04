using System;
using System.Reflection;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks the supplied type contains the supplied fields and those fields
        /// are not marked with a JsonIgnore attribute
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static bool TypeHasOutputProperties(this Type type, string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the field are separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // check if the requested fields exist on source
            foreach (var field in fieldsAfterSplit)
            {
                // trim each field, as it might contain leading 
                // or trailing spaces. Can't trim the var in foreach,
                // so use another var.
                var propertyName = field.Trim();

                // use reflection to check if the property can be
                // found on T. 
                var propertyInfo = type
                    .GetProperty(propertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // it can't be found, return false
                if (propertyInfo == null)
                {
                    return false;
                }

                if (propertyInfo.HasSerializationIgnoreAttribute())
                {
                    return false;
                }
            }

            // all checks out, return true
            return true;
        }
    }
}