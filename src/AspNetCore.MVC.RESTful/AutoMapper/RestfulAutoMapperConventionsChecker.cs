using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace AspNetCore.MVC.RESTful.AutoMapper
{
    /// <summary>
    /// Checks whether the require Mappings have been registered for the 
    /// AddRestful endpoints (Get, Get (collection), Create, Update)
    /// </summary>
    public class RestfulAutoMapperConventionsChecker
    {
        private readonly IMapper _mapper;

        public RestfulAutoMapperConventionsChecker(IMapper mapper)
        {
            _mapper = mapper;
        }

        public void CheckReadonly<TEntity>()
        {
            Check<TEntity>(
                checkResourceGet: true,
                checkResourcesGet: true,
                checkResourceCreate: false,
                checkResourceUpdate: false);
        }


        public void Check<TEntity>(
                bool checkResourceGet = true,
                bool checkResourcesGet = true,
                bool checkResourceCreate = true,
                bool checkResourceUpdate = true
            )
        {
            var allTypeMaps = _mapper.ConfigurationProvider.GetAllTypeMaps()
                .Select(m => (m.SourceType.Name, m.DestinationType.Name));

            var entity = typeof(TEntity).Name;

            var getMappings = new List<(string, string)>()
            {
                // Base mapping from entity to dto
                ($"{entity}", $"{entity}Dto"),
            };

            var getCollectionMappings = new List<(string, string)>()
                {
                // GET /{resources} parameters filters
                ( $"{Parameters(entity)}", $"{Filters(entity)}" ),
                ( $"{Filters(entity)}", $"{Parameters(entity)}" ),

                // GET /{resources} parameters search query
                ( $"{Parameters(entity)}", $"{Search(entity)}" ),
                ( $"{Search(entity)}", $"{Parameters(entity)}" ),

                // GET /{resources} parameters pagination
                ( $"{Parameters(entity)}", $"PaginationParameters" ),
                ( $"PaginationParameters", $"{Parameters(entity)}" ),

                // GET /{resources} parameters order by
                ( $"{Parameters(entity)}", $"OrderByParameters" ),
                ( $"OrderByParameters", $"{Parameters(entity)}" ),
            };

            var createMappings = new List<(string, string)>
            {
                // POST /{resource}
                ( $"{entity}CreationDto", $"{entity}"),
            };

            var updateMappings = new List<(string, string)>
            {
                // PUT /{resource}
                // PATCH /{resource}
                ( $"{entity}UpdateDto", $"{entity}"),
                ( $"{entity}", $"{entity}UpdateDto"),
            };

            foreach (var typeMapping in allTypeMaps)
            {
                getMappings.Remove(typeMapping);
                getCollectionMappings.Remove(typeMapping);
                createMappings.Remove(typeMapping);
                updateMappings.Remove(typeMapping);
            }

            CheckMappingsFor($"{entity}:ResourceGet", getMappings, checkResourceGet);
            CheckMappingsFor($"{entity}:ResourcesGet", getCollectionMappings, checkResourcesGet);
            CheckMappingsFor($"{entity}:ResourceCreate", createMappings, checkResourceCreate);
            CheckMappingsFor($"{entity}:ResourceUpdate", updateMappings, checkResourceUpdate);
        }

        private static void CheckMappingsFor(string method, List<(string, string)> mappings, bool check = true)
        {
            if (check && mappings.Any())
            {
                throw new AutoMapperMappingException($"{method} is missing the following mappings:\n" +
                    $"\t{string.Join("\n\t", mappings).Replace(",","->", StringComparison.InvariantCultureIgnoreCase)}");
            }
        }

        private static string Filters(string entity) => $"Get{entity}sFilters";
        private static string Parameters(string entity) => $"Get{entity}sParameters";
        private static string Search(string entity) => $"Get{entity}sSearchQuery";
    }
}