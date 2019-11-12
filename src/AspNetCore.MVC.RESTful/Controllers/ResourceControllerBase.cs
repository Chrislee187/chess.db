﻿using System;
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
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Models;
using AspNetCore.MVC.RESTful.Repositories;
using AspNetCore.MVC.RESTful.Services;

namespace AspNetCore.MVC.RESTful.Controllers
{
    /// <summary>
    /// Abstract implementation of an MVC Controller that supports default standard REST endpoints.
    /// i.e. HEAD, OPTIONS, GET, POST, PUT, PATCH, DELETE
    /// <list>
    /// See also 
    /// <seealso cref="RestfulAutoMapperConventionsChecker"></seealso>,
    /// <seealso cref="HateoasController"></seealso>.
    /// <seealso cref="IResourceRepository{TEntity,TId}"></seealso>,
    /// <seealso cref="IEntityUpdater{TEntity,TId}"></seealso>,
    /// <seealso cref="IOrderByPropertyMappingService{TEntity,TId}"></seealso>
    /// </list>
    /// </summary>

    /// <typeparam name="TDto">Data Transfer Object that can be "Automapped" to and from <see cref="TEntity"/></typeparam>
    /// <typeparam name="TEntity">Underlying Entity for the Resource being represented</typeparam>
    /// <typeparam name="TId">Underlying type of the primary key "Id" field of the <see cref="TEntity"/></typeparam>
    public abstract class ResourceControllerBase<TDto, TEntity, TId> : HateoasController<TEntity, TId>
        where TEntity : class
        where TDto : IResourceId<Guid>
    {
        private readonly IResourceRepository<TEntity, TId> _restResourceRepository;
        private readonly IOrderByPropertyMappingService<TDto, TEntity> _orderByPropertyMappingService;
        private readonly IEntityUpdater<TEntity, TId> _entityUpdater;

        protected IMapper Mapper { get; }

        protected ResourceControllerBase(
            [NotNull] IMapper mapper,
            [NotNull] IResourceRepository<TEntity, TId> resourceRepository,
            IOrderByPropertyMappingService<TDto, TEntity> orderByPropertyMappingService,
            IEntityUpdater<TEntity, TId> entityUpdater,
            Action<HateoasConfig> config = null)
                :base()
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
            [NotNull] IEntityFilter<TEntity> entityFilter,
            [NotNull] IEntitySearch<TEntity> entitySearch,
            IEnumerable<HateoasLink> additionalCollectionLinks = null,
            IEnumerable<HateoasLink> additionalIndividualLinks = null)
        {
            if (!typeof(TDto).TypeHasOutputProperties(CollectionConfig.Shape))
            {
                return BadRequest("Shape has one or more invalid field names.");
            }

            var orderByCheck = _orderByPropertyMappingService
                .ClauseIsValid(CollectionConfig.OrderBy);

            if (!orderByCheck.Valid)
            {
                orderByCheck.Details.Instance = Url.Link(HateoasConfig.ResourcesGetRouteName, parameters);
                return BadRequest(orderByCheck.Details);
            }

            var orderByMappings = _orderByPropertyMappingService.GetPropertyMapping();

            var pagedEntities = _restResourceRepository
                .Load(
                    CollectionConfig.Page,
                    CollectionConfig.PageSize, 
                    entityFilter, 
                    entitySearch,
                    CollectionConfig.SearchText,
                    CollectionConfig.OrderBy, 
                    orderByMappings);

            AddPaginationHeader(HateoasConfig.ResourcesGetRouteName, pagedEntities);

            var resources = Mapper.Map<IEnumerable<TDto>>(pagedEntities)
                .ShapeData(CollectionConfig.Shape).ToList();
            
            if (HateoasConfig.AddLinksToIndividualResources)
            {
                AddHateoasLinksToResourceCollection(resources, additionalIndividualLinks);
            }
            
            if (HateoasConfig.AddLinksToCollectionResources)
            {
                additionalCollectionLinks = additionalCollectionLinks == null
                    ? new List<HateoasLink>().ToList()
                    : additionalCollectionLinks.ToList();
                
                var collectionLinks = ResourcesGetLinks(parameters, pagedEntities);
                AddCustomLinks(collectionLinks, additionalCollectionLinks);

                var expandoObject = new {value = resources}.ShapeData("");
                expandoObject.TryAdd(HateoasConfig.LinksPropertyName, collectionLinks);
                
                return Ok(expandoObject);
            }

            return Ok(resources);
        }


        /// <summary>
        /// HTTP GET /{resource}/{id}
        /// </summary>
        protected ActionResult<TDto> ResourceGet(TId id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            if (!typeof(TDto).TypeHasOutputProperties(CollectionConfig.Shape))
            {
                return BadRequest("Shape has one or more invalid field names.");
            }

            var entity = _restResourceRepository.Load(id);

            if (entity == null)
            {
                return NotFound();
            }

            var resource = (IDictionary<string, object>) Mapper.Map<TDto>(entity)
                .ShapeData(CollectionConfig.Shape);

            if (HateoasConfig.AddLinksToIndividualResources)
            {
                if (resource.ContainsKey("Id"))
                {
                    resource.Add(HateoasConfig.LinksPropertyName, ResourceGetLinks(id, CollectionConfig.Shape, additionalLinks));
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
            [NotNull] TCreationDto model,
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
                dynamic id = createdResource.Id;
                resource.Add(HateoasConfig.LinksPropertyName, ResourceCreateLinks(id, additionalLinks));
            }

            return CreatedAtRoute(
                HateoasConfig.ResourcesGetRouteName,
                new {createdResource.Id},
                resource.ShapeData(CollectionConfig.Shape)
            );
        }

        /// <summary>
        /// HTTP PUT
        /// </summary>
        protected ActionResult ResourceUpsert<TUpdateDto>(
            TId id,
            [NotNull] TUpdateDto model,
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
                    resource.Add(HateoasConfig.LinksPropertyName, ResourceUpsertLinks(id, additionalLinks));
                }

                result = CreatedAtRoute(
                    HateoasConfig.ResourceGetRouteName,
                    new {id},
                    resource.ShapeData(CollectionConfig.Shape)
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
        protected ActionResult ResourcePatch<TUpdateDto>(TId id,
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

                resourceForLinks.Add(HateoasConfig.LinksPropertyName, ResourcePatchLinks(id));
                return Ok(resourceForLinks.ShapeData(CollectionConfig.Shape));
            }

            return NoContent();
        }

        /// <summary>
        /// HTTP DELETE
        /// </summary>
        protected ActionResult ResourceDelete(TId id)
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

        private void AddPaginationHeader(string resourcesGetRouteName,
            [NotNull] IPaginationMetadata pagedEntities) 
        {
            var xPaginationHeader = new XPaginationHeader(
                pagedEntities,
                (parameters) => Url.Link(resourcesGetRouteName, parameters),
                CollectionConfig
            );
            Response.Headers.Add(xPaginationHeader.KVP);
        }
    }
}