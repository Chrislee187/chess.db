using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace AspNetCore.MVC.RESTful.AutoMapper
{
    /// <summary>
    /// Checks whether the require Mappings have been registered for the 
    /// RESTful endpoints (Get, Get (collection), Create, Update)
    /// </summary>
    public class AutoMapperConventionsChecker
    {
        private IMapper _mapper;

        public string Dto = "Dto";
        public AutoMapperConventionsChecker(IMapper mapper)
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
        private string Filters(string entity) => $"Get{entity}sFilters";
        private string Parameters(string entity) => $"Get{entity}sParameters";
        private string Search(string entity) => $"Get{entity}sSearchQuery";

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
                ($"{entity}", $"{entity}{Dto}"),
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

            CheckMappingsFor(checkResourceGet,$"{entity}:ResourceGet", getMappings);
            CheckMappingsFor(checkResourcesGet, $"{entity}:ResourcesGet", getCollectionMappings);
            CheckMappingsFor(checkResourceCreate,$"{entity}:ResourceCreate", createMappings);
            CheckMappingsFor(checkResourceUpdate,$"{entity}:ResourceUpdate", updateMappings);
        }

        private void CheckMappingsFor(bool check, string method, List<(string, string)> mappings)
        {
            if (check && mappings.Any())
            {
                throw new AutoMapperMappingException($"{method} is missing the following mappings:\n" +
                                                     $"{string.Join("\n", mappings)}");
            }
        }
    }
}