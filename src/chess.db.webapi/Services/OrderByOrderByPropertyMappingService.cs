using System;
using System.Collections.Generic;
using System.Linq;
using chess.db.webapi.Models;
using chess.games.db.api.Helpers;
using chess.games.db.Entities;

namespace chess.db.webapi.Services
{
    /// <summary>
    /// Simple service that maps property names to field names
    /// </summary>
    public class OrderByOrderByPropertyMappingService : IOrderByPropertyMappingService
    {
        private static readonly Dictionary<string, OrderByPropertyMappingValue> Mappings =
          new Dictionary<string, OrderByPropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
          {
               { "Lastname", new OrderByPropertyMappingValue(new List<string>() { "Surname" } ) }
          };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public OrderByOrderByPropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<PlayerDto, Player>(Mappings));
        }

        public bool ClauseIsValid<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    var propertyInfo = typeof(TSource).GetProperties().Any(p => p.Name.ToLowerInvariant().Equals(propertyName));
                    if (!propertyInfo)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        public Dictionary<string, OrderByPropertyMappingValue> GetPropertyMapping
           <TSource, TDestination>()
        {
            var mapping = _propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>()
                .ToList();

            if (mapping.Any())
            {
                return mapping.First().MappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }
    }
}