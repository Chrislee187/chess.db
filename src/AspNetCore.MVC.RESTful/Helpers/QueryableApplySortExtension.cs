using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using AspNetCore.MVC.RESTful.Parameters;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public static class QueryableApplySortExtension
    {
        /// <summary>
        /// String based sorting using form
        ///
        /// 'fieldname [desc|asc], [[fieldname [desc|asc]] ...'
        /// </summary>
        /// <remarks>
        /// Uses `System.Linq.Dynamic.Core` for the OrderBy(string) Linq extension
        /// </remarks>
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, 
            string orderBy, 
            IDictionary<string, OrderByPropertyMappingValue> mappingDictionary)
        {
            NullX.Throw(source, nameof(source));

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            mappingDictionary ??= new Dictionary<string, OrderByPropertyMappingValue>();

            foreach (var orderByClause in orderBy
                .Split(',').Reverse()
                .Select(o => o.Trim()))
            {
                var orderDescending = IsOrderDescending(orderByClause);
                var propertyName = PropertyName(orderByClause);

                if (!mappingDictionary.TryGetValue(propertyName, out var propertyMappingValue))
                {
                    propertyMappingValue = new OrderByPropertyMappingValue(new[] { propertyName});
                }

                foreach (var destinationProperty in
                    propertyMappingValue.DestinationProperties.Reverse())
                {
                    if (propertyMappingValue.Reverse)
                    {
                        orderDescending = !orderDescending;
                    }

                    source = source.OrderBy(destinationProperty +
                        (orderDescending ? " descending" : " ascending"));
                }
            }

            return source;

            bool IsOrderDescending(string orderByClause)
            {
                return orderByClause.EndsWith(" desc", StringComparison.InvariantCulture)
                       || orderByClause.EndsWith(" descending", StringComparison.InvariantCulture);
            }

            string PropertyName(string orderByClause)
            {
                var idx = orderByClause.IndexOf(" ", StringComparison.Ordinal);
                return idx == -1 ? orderByClause : orderByClause.Remove(idx);
            }
        }
    }
}