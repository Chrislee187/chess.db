using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AutoMapper;
using AspNetCore.MVC.RESTful.AutoMapper;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Models;
using AspNetCore.MVC.RESTful.Parameters;
using AspNetCore.MVC.RESTful.Repositories;

namespace AspNetCore.MVC.RESTful.Controllers
{
    /// <summary>
    /// A base class for an MVC Controller that supports Restful endpoints.
    /// Relies heavily on AutoMapper mapping abilities to allow reuse
    /// <list>
    /// See also 
    /// <seealso cref="RestfulAutoMapperConventionsChecker"></seealso>,
    /// <seealso cref="HateoasController"></seealso>
    /// </list>
    /// </summary>

    /// <typeparam name="TDto">Data Transfer Object that can be Automapped from TEntity</typeparam>
    /// <typeparam name="TEntity">Underlying Entity for the Resource being represented</typeparam>
    public abstract class ResourceControllerBase<TDto, TEntity> : HateoasController
        where TEntity : class
        where TDto : IResourceId
    {
        private readonly IResourceRepository<TEntity> _restResourceRepository;
        private readonly IOrderByPropertyMappingService<TDto, TEntity> _orderByPropertyMappingService;
        private readonly IEntityUpdater<TEntity> _entityUpdater;

        protected IMapper Mapper { get; }

        protected ResourceControllerBase(IMapper mapper,
            IResourceRepository<TEntity> resourceRepository,
            IOrderByPropertyMappingService<TDto, TEntity> orderByPropertyMappingService,
            IEntityUpdater<TEntity> entityUpdater,
            Action<HateoasConfig> config = null)
                :base(nameof(TEntity))
        {
            ConfigureHateoas(config);

            Mapper = NullX.Throw(mapper, nameof(mapper));
            _restResourceRepository = NullX.Throw(resourceRepository, nameof(resourceRepository));
            _orderByPropertyMappingService = NullX.Throw(orderByPropertyMappingService, nameof(orderByPropertyMappingService));
            _entityUpdater = NullX.Throw(entityUpdater, nameof(entityUpdater));
        }

        /// <summary>
        /// HTTP GET /{resource}
        /// </summary>
        protected IActionResult ResourcesGet<TParameters>([NotNull] TParameters parameters,
            [NotNull] IResourceQuery<TEntity> filters,
            [NotNull] IResourceQuery<TEntity> resourceQuery,
            IEnumerable<HateoasLink> additionalCollectionLinks = null,
            IEnumerable<HateoasLink> additionalIndividualLinks = null)
            where TParameters : CommonResourcesGetParameters
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
                orderByCheck.Details.Instance = Url.Link(HateoasConfig.ResourceGetRouteName.Get(), parameters);
                return BadRequest(orderByCheck.Details);
            }

            var pagination = Mapper.Map<PaginationParameters>(parameters);

            var orderByMappings = _orderByPropertyMappingService.GetPropertyMapping();

            var pagedEntities = _restResourceRepository.Load(filters, resourceQuery, pagination, orderBy, orderByMappings);

            var usedParameters = RecreateCollectionParameters(parameters, filters, resourceQuery, pagination, orderBy);

            AddPaginationHeader(HateoasConfig.ResourceGetRouteName.Get(), pagedEntities, usedParameters);

            var resources = Mapper.Map<IEnumerable<TDto>>(pagedEntities).ShapeData(parameters.Shape).ToList();
            
            if (HateoasConfig.AddLinksToIndividualResources)
            {
                AddHateoasLinksToResourceCollection(resources, usedParameters, additionalIndividualLinks);
            }

            additionalCollectionLinks = additionalCollectionLinks == null
                ? new List<HateoasLink>().ToList()
                : additionalCollectionLinks.ToList();
            if (HateoasConfig.AddLinksToCollectionResources)
            {
                var collectionLinks = ResourcesGetLinks(parameters, pagedEntities);
                AddCustomLinks(collectionLinks, additionalCollectionLinks);

                return Ok(new
                {
                    value = resources,
                    links = collectionLinks
                });
            }

            AddCustomLinks(new List<HateoasLink>(), additionalCollectionLinks);

            return Ok(resources);
        }


        /// <summary>
        /// HTTP GET /{resource}/{id}
        /// </summary>
        protected ActionResult<TDto> ResourceGet(Guid id, string shape,
            IEnumerable<HateoasLink> additionalLinks = null)
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

            shape ??= "";

            var resource = (IDictionary<string, object>) Mapper.Map<TDto>(entity)
                .ShapeData(shape);

            if (HateoasConfig.AddLinksToIndividualResources)
            {
                if (resource.ContainsKey("Id"))
                {
                    resource.Add("_links", ResourceGetLinks(id, shape, additionalLinks));
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
            IEnumerable<HateoasLink> additionalLinks = null
        )
        {
            var entity = Mapper.Map<TEntity>(model);

            _restResourceRepository.Add(entity);
            _restResourceRepository.Save();

            var createdResource = Mapper.Map<TDto>(entity);

            IDictionary<string, object> resource = createdResource.ShapeData("");
            
            if (HateoasConfig.AddLinksToIndividualResources)
            {
                resource.Add("_links", ResourceCreateLinks(createdResource.Id, additionalLinks));
            }

            return CreatedAtRoute(
                HateoasConfig.ResourcesGetRouteName.Get(),
                new {createdResource.Id},
                resource
            );
        }

        /// <summary>
        /// HTTP PUT
        /// </summary>
        protected ActionResult ResourceUpsert<TUpdateDto>(
            Guid id,
            TUpdateDto model,
            IEnumerable<HateoasLink> additionalLinks = null)
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

                IDictionary<string, object> resource = createdDto.ShapeData("");

                if (HateoasConfig.AddLinksToIndividualResources)
                {
                    resource.Add("_links", ResourceUpsertLinks(id, additionalLinks));
                }

                result = CreatedAtRoute(
                    HateoasConfig.ResourceGetRouteName.Get(),
                    new {id},
                    resource
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

            if (HateoasConfig.AddLinksToIndividualResources)
            {
                IDictionary<string, object> resourceForLinks = patchedResource.ShapeData("");

                resourceForLinks.Add("_links", ResourcePatchLinks(id));
                return Ok(resourceForLinks);
            }

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

        private void AddPaginationHeader<TParameters>(string resourcesGetRouteName, 
            IPaginationMetadata pagedEntities,
            TParameters usedParameters) where TParameters : CommonResourcesGetParameters
        {
            var xPaginationHeader = new XPaginationHeader(
                pagedEntities,
                usedParameters,
                (parameters) => Url.Link(resourcesGetRouteName, parameters)
            );
            Response.Headers.Add(xPaginationHeader.Key, xPaginationHeader.Value);
        }

        private TParameters RecreateCollectionParameters<TParameters>(TParameters parameters, IResourceQuery<TEntity> filters,
            IResourceQuery<TEntity> resourceQuery, PaginationParameters pagination, OrderByParameters orderBy)
            where TParameters : CommonResourcesGetParameters
        {
            var usedParameters = Activator.CreateInstance<TParameters>();

            Mapper.Map(pagination, usedParameters);
            Mapper.Map(orderBy, usedParameters);
            Mapper.Map(filters, usedParameters);
            Mapper.Map(resourceQuery, usedParameters);
            usedParameters.Shape = parameters.Shape;
            return usedParameters;
        }
    }
}