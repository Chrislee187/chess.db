using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public static class QueryCollectionExtensions
    {
        public static string GetByAlias(this IQueryCollection query, params string[] aliases)
        {
            var key = query.Keys.SingleOrDefault(k => aliases.Select(a => a.ToLowerInvariant()).Contains(k.ToLowerInvariant()));
            return key == null 
                ? null 
                : (string)query[key];
        }
    }
}