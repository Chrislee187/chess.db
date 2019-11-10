using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public static class QueryCollectionExtensions
    {
        public static bool ContainsArg(this IQueryCollection query, string argName)
        {
            return query.Keys.Any(k => k.Equals(argName, StringComparison.OrdinalIgnoreCase));
        }
        public static string ArgValue(this IQueryCollection query, params string[] argNames)
        {
            var key = query.Keys.SingleOrDefault(k => argNames.Select(a => a.ToLowerInvariant()).Contains(k.ToLowerInvariant()));
            return key == null 
                ? null 
                : (string)query[key];
        }
    }
}