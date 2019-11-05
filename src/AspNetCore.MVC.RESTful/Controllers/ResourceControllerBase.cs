using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using AspNetCore.MVC.RESTful.AutoMapper;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Models;
using AspNetCore.MVC.RESTful.Parameters;
using AspNetCore.MVC.RESTful.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AspNetCore.MVC.RESTful.Controllers
{
    /// <summary>
    /// A base class for an MVC Controller that supports Restful endpoints.
    /// Relies heavily on Automapper mapping abilities to Generic'ify the process for simple reuse
    /// <seealso cref="RestfulAutoMapperConventionsChecker"></seealso>
    /// </summary>
    /// <typeparam name="TDto">Data Transfer Object that can be Automapped from TEntity</typeparam>
    /// <typeparam name="TEntity">Underlying Entity for the Resource being represented</typeparam>
    public abstract class ResourceControllerBase<TDto, TEntity> : ControllerBase
        where TEntity : class
        where TDto : IResourceId
    {
        private readonly IResourceRepository<TEntity> _restResourceRepository;
        private readonly IOrderByPropertyMappingService<TDto, TEntity> _orderByPropertyMappingService;
        private readonly IEntityUpdater<TEntity> _entityUpdater;

        private readonly ResourceControllerConfig _controllerConfig;

        protected ResourceControllerBase(IMapper mapper,
            IResourceRepository<TEntity> resourceRepository,
            IOrderByPropertyMappingService<TDto, TEntity> orderByPropertyMappingService,
            IEntityUpdater<TEntity> entityUpdater,
            Action<ResourceControllerConfig> config = null)
        {
            _controllerConfig = new ResourceControllerConfig();
            config?.Invoke(_controllerConfig);

            Mapper = NullX.Throw(mapper, nameof(mapper));
            _restResourceRepository = NullX.Throw(resourceRepository, nameof(resourceRepository));
            _orderByPropertyMappingService = NullX.Throw(orderByPropertyMappingService, nameof(orderByPropertyMappingService));
            _entityUpdater = NullX.Throw(entityUpdater, nameof(entityUpdater));

        }

        protected IMapper Mapper { get; }

        /// <summary>
        /// HTTP GET /{resource}
        /// </summary>
        protected IActionResult ResourcesGet<TParameters>([NotNull] TParameters parameters,
            [NotNull] IResourceQuery<TEntity> filters,
            [NotNull] IResourceQuery<TEntity> resourceQuery,
            string thisRouteName,
            string resourceGetRouteName = null,
            IEnumerable<HateoasLink> additionalLinks = null)
            where TParameters : CommonResourceParameters
        {
            if (!typeof(TDto).TypeHasOutputProperties(parameters.Shape))
            {
                return BadRequest("Shape has one or more invalid field names.");
            }

            var orderBy = Mapper.Map<OrderByParameters>(parameters);

            var orderByCheck = _orderByPropertyMappingService
                .ClauseIsValid(orderBy.Clause);

            if (!orderByCheck.Valid)
            {
                orderByCheck.Details.Instance = Url.Link(thisRouteName, parameters);
                return BadRequest(orderByCheck.Details);
            }

            var pagination = Mapper.Map<PaginationParameters>(parameters);

            var orderByMappings = _orderByPropertyMappingService.GetPropertyMapping();

            var pagedEntities = _restResourceRepository
                .Load(filters, resourceQuery, pagination, orderBy, orderByMappings);

            var usedParameters = Activator.CreateInstance<TParameters>();

            Mapper.Map(pagination, usedParameters);
            Mapper.Map(orderBy, usedParameters);
            Mapper.Map(filters, usedParameters);
            Mapper.Map(resourceQuery, usedParameters);
            usedParameters.Shape = parameters.Shape;

            AddPaginationHeader(thisRouteName, pagedEntities, usedParameters);

            var resources = Mapper.Map<IEnumerable<TDto>>(pagedEntities)
                .ShapeData(parameters.Shape).ToList();

            var linkBuilder = new ResourcesGetLinksBuilder<TParameters>(
                o => Url.Link(thisRouteName, o)
            );
            object result = resources;

            if (_controllerConfig.Hateoas.AddHateoasLinks)
            {
                AddHateoasLinksToResources(resourceGetRouteName, resources, usedParameters);

                var collectionLinks = linkBuilder
                    .ResourcesGetLinks(usedParameters, pagedEntities, additionalLinks);

                AddCustomLinks(additionalLinks, collectionLinks);

                result = new
                {
                    value = resources,
                    links = collectionLinks
                };
            }

            return Ok(result);
        }

        /// <summary>
        /// HTTP GET /{resource}/{id}
        /// </summary>
        protected ActionResult<TDto> ResourceGet(Guid id, string shape,
            string thisRouteName)
        {
            if (!typeof(TDto).TypeHasOutputProperties(shape))
            {
                return BadRequest("Shape has one or more invalid field names.");
            }

            var entity = _restResourceRepository.Load(id);

            if (entity == null)
            {
                return NotFound();
            }

            var resource = (IDictionary<string, object>) Mapper.Map<TDto>(entity).ShapeData(shape);

            if (_controllerConfig.Hateoas.AddHateoasLinks)
            {
                var linkBuilder = new ResourceGetLinksBuilder<Guid>(
                    (id2, shape2) => Url.Link(thisRouteName, new { id = id2, shape = shape2 })
                );
                    
                if (resource.TryGetValue("Id", out var idObj))
                {
                    var resourceLinks = linkBuilder.ResourceGetLinks(
                        new Guid(idObj.ToString()),
                        shape);

                    // AddCustomChildLinks(additionalLinks, resourceLinks);
                    resource.Add("links", resourceLinks);
                }
            }

            return Ok(resource);
        }

        /// <summary>
        /// HTTP POST - returns CreatedAtRoute (201 - Created) and places a 'Location' entry in
        /// the response header containing the uri to retrieve the newly added resource
        /// created from the supplied args, also returns the newly created resource in
        /// the body
        /// </summary>
        protected ActionResult<TDto> ResourceCreate<TCreationDto>(
            TCreationDto model,
            string resourceGetRouteName
        )
        {
            var entity = Mapper.Map<TEntity>(model);

            _restResourceRepository.Add(entity);
            _restResourceRepository.Save();

            var createdResource = Mapper.Map<TDto>(entity);

            return CreatedAtRoute(
                resourceGetRouteName,
                new {createdResource.Id},
                createdResource
            );
        }

        /// <summary>
        /// HTTP PUT
        /// </summary>
        protected ActionResult ResourceUpsert<TUpdateDto>(
            Guid id,
            TUpdateDto model,
            string resourceGetRouteName)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }

            var entity = _restResourceRepository.Load(id);

            ActionResult result;
            if (entity == null)
            {
                var addedEntity = Mapper.Map<TEntity>(model);
                _entityUpdater.SetId(addedEntity, id);
                _restResourceRepository.Add(addedEntity);

                var createdDto = Mapper.Map<TDto>(addedEntity);

                result = CreatedAtRoute(
                    resourceGetRouteName,
                    new {id},
                    createdDto
                );
            }
            else
            {
                Mapper.Map(model, entity);
                _restResourceRepository.Update(entity);
                result = NoContent();
            }

            _restResourceRepository.Save();

            return result;
        }

        /// <summary>
        /// HTTP PATCH
        /// </summary>
        protected ActionResult ResourcePatch<TUpdateDto>(Guid id,
            [NotNull] JsonPatchDocument<TUpdateDto> patchDocument)
            where TUpdateDto : class
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }

            var resource = _restResourceRepository.Load(id);
            if (resource == null)
            {
                return NotFound();
            }

            var patchedResource = Mapper.Map<TUpdateDto>(resource);
            patchDocument.ApplyTo(patchedResource);

            if (!TryValidateModel(patchedResource))
            {
                return ValidationProblem(ModelState);
            }

            Mapper.Map(patchedResource, resource);

            _restResourceRepository.Update(resource);
            _restResourceRepository.Save();

            return NoContent();
        }

        /// <summary>
        /// HTTP DELETE
        /// </summary>
        protected ActionResult ResourceDelete(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }

            var resource = _restResourceRepository.Load(id);

            if (resource == null)
            {
                return NotFound();
            }

            _restResourceRepository.Delete(resource);
            _restResourceRepository.Save();

            return NoContent();
        }

        /// <summary>
        /// HTTP OPTIONS
        /// </summary>
        protected IActionResult ResourceOptions(params string[] httpMethods)
        {
            Response.Headers.Add("Allow", string.Join(',', httpMethods));
            return Ok();
        }

        /// <summary>
        /// Creates an <seealso cref="ActionResult"></seealso> generated by the default invalid model state response handler.
        /// </summary>
        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult) options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        /// <summary>
        /// Add 'X-Pagination' header containing pagination meta data
        /// </summary>
        protected void AddPaginationMetadataHeader<T>(
            [NotNull] PagedList<T> data,
            [NotNull] IResourceUris urls)
        {
            var paginationMetadata = new
            {
                totalCount = data.TotalCount,
                pageSize = data.PageSize,
                currentPage = data.CurrentPage,
                totalPages = data.TotalPages,
                previousPageLink = urls.Previous,
                nextPageLink = urls.Next
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata,
                new JsonSerializerOptions()
                {
                    // NOTE: Stops the '?' & '&' chars in the links being escaped
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));

        }

        private static void AddCustomLinks(IEnumerable<HateoasLink> additionalLinks, List<HateoasLink> links)
        {
            if (additionalLinks != null)
            {
                links.AddRange(additionalLinks);
            }
        }

        private void AddPaginationHeader<TParameters>(string resourcesGetRouteName, 
            IPaginationMetadata pagedEntities,
            TParameters usedParameters) where TParameters : CommonResourceParameters
        {
            var xPaginationHeader = new XPaginationHeader(
                pagedEntities,
                usedParameters,
                (parameters) => Url.Link(resourcesGetRouteName, parameters)
            );
            Response.Headers.Add(xPaginationHeader.Key, xPaginationHeader.Value);
        }

        private void AddHateoasLinksToResources<TParameters>(string resourceGetRouteName, 
            IEnumerable<ExpandoObject> resources, 
            TParameters usedParameters)
            where TParameters : CommonResourceParameters
        {
            // NOTE: Hateoas "links" support is only available if the ID is available, if
            // the data has been reshaped to not include the Id, no links will be added.

            var linkBuilder = new ResourceGetLinksBuilder<Guid>(
                (id, shape) => Url.Link(resourceGetRouteName, new { id, shape })
            );

            foreach (IDictionary<string, object> resource in resources)
            {
                if (resource.TryGetValue("Id", out var idObj))
                {
                    var resourceLinks = linkBuilder.ResourceGetLinks(
                        new Guid(idObj.ToString()),
                        usedParameters.Shape);
                    
                    //                            AddCustomChildLinks(additionalLinks, resourceLinks);
                    resource.Add("links", resourceLinks);
                }
            }
        }
    }

    public class ResourceControllerConfig
    {
        public HateoasConfig Hateoas { get; } = new HateoasConfig();
    }

    public class HateoasConfig
    {
        public bool AddHateoasLinks { get; set; } = true;
    }
}