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
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            mappingDictionary ??= new Dictionary<string, OrderByPropertyMappingValue>();

            var orderByAfterSplit = orderBy.Split(',');

            foreach (var orderByClause in orderByAfterSplit.Reverse().Select(o => o.Trim()))
            {
                var orderDescending = orderByClause.EndsWith(" desc") || orderByClause.EndsWith(" descending");
                var indexOfFirstSpace = orderByClause.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    orderByClause : orderByClause.Remove(indexOfFirstSpace);

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
        }
    }
}