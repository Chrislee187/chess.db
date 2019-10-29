using System;
using System.Collections.Generic;
using chess.games.db.api.Helpers;

namespace chess.db.webapi.Services
{
    // ReSharper disable once UnusedTypeParameter
    // NOTE: Generic types here are used as keys to differentiate between mappings
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, OrderByPropertyMappingValue> MappingDictionary { get; private set; }

        public PropertyMapping(Dictionary<string, OrderByPropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary ??
                                throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}