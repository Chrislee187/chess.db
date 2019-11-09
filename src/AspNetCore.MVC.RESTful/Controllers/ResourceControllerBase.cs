using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
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
    /// Relies heavily on AutoMapper mapping abilities to allow reuse
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

        protected readonly HateoasConfig HateoasConfig;

        protected ResourceControllerBase(IMapper mapper,
            IResourceRepository<TEntity> resourceRepository,
            IOrderByPropertyMappingService<TDto, TEntity> orderByPropertyMappingService,
            IEntityUpdater<TEntity> entityUpdater,
            Action<HateoasConfig> config = null)
        {
            HateoasConfig = new HateoasConfig();
            config?.Invoke(HateoasConfig);

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
            IEnumerable<HateoasLink> additionalCollectionLinks = null,
            IEnumerable<HateoasLink> additionalIndividualLinks = null)
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
                orderByCheck.Details.Instance = Url.Link(HateoasConfig.ResourceGetRouteName.Get(), parameters);
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

            AddPaginationHeader(HateoasConfig.ResourceGetRouteName.Get(), pagedEntities, usedParameters);

            var resources = Mapper.Map<IEnumerable<TDto>>(pagedEntities)
                .ShapeData(parameters.Shape).ToList();
            
            if (HateoasConfig.AddDefaultLinksToIndividualResources)
            {
                AddHateoasLinksToResources(resources, usedParameters, additionalIndividualLinks);
            }

            additionalCollectionLinks = additionalCollectionLinks == null
                ? new List<HateoasLink>().ToList()
                : additionalCollectionLinks.ToList();
            if (HateoasConfig.AddDefaultLinksToCollectionResources)
            {
                var collectionLinks = ResourcesGetLinks(parameters, pagedEntities);
                AddCustomLinks(additionalCollectionLinks, collectionLinks);

                return Ok(new
                {
                    value = resources,
                    links = collectionLinks
                });
            }

            AddCustomLinks(additionalCollectionLinks, new List<HateoasLink>());

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

            if (HateoasConfig.AddDefaultLinksToIndividualResources)
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
            
            if (HateoasConfig.AddDefaultLinksToIndividualResources)
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

                if (HateoasConfig.AddDefaultLinksToIndividualResources)
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

            if (HateoasConfig.AddDefaultLinksToIndividualResources)
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

        private HateoasLink ResourcesGetLinkBuilder(string rel, object parameters)
            => new HateoasLink(
                rel,
                "GET",
                Url.Link(HateoasConfig.ResourcesGetRouteName.Get(), parameters)
            );

        private List<HateoasLink> ResourcesGetLinks<TParameters>(
            TParameters parameters,
            IPaginationMetadata pagination)
            where TParameters : CommonResourceParameters
        {
            var links = new List<HateoasLink>
            {
                ResourcesGetLinkBuilder("current-page", parameters)
            };

            if (pagination.HasPrevious)
            {
                parameters.PageNumber = pagination.CurrentPage - 1;
                links.Add(ResourcesGetLinkBuilder("prev-page", parameters));
            }

            if (pagination.HasNext)
            {
                parameters.PageNumber = pagination.CurrentPage + 1;
                links.Add(ResourcesGetLinkBuilder("next-page", parameters));
            }

            parameters.PageNumber = pagination.CurrentPage;

            return links;
        }

        public List<HateoasLink> ResourceGetLinks(Guid id, string shape,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder("self", id, shape),
                ResourceUpsertLinkBuilder(id),
                ResourcePatchLinkBuilder(id),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(additionalLinks, links);

            return links;
        }
        public List<HateoasLink> ResourceCreateLinks(Guid id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder("self", id, ""),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(additionalLinks, links);

            return links;
        }

        public List<HateoasLink> ResourceUpsertLinks(Guid id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder("self", id, ""), 
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(additionalLinks, links);

            return links;
        }

        public List<HateoasLink> ResourcePatchLinks(Guid id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder("self", id, ""),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(additionalLinks, links);

            return links;
        }

        public List<HateoasLink> ResourceDeleteLinks(IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourcesGetLinkBuilder("current-page", null),
                ResourceCreateLinkBuilder()
            };

            AddCustomLinks(additionalLinks, links);

            return links;
        }
        private HateoasLink ResourceGetLinkBuilder(string rel, Guid id, string shape)
        {
            var s = string.IsNullOrEmpty(shape) ? "" : $"?shape={shape}";

            return new HateoasLink(
                rel,
                "GET",
                $"{Url.Link(HateoasConfig.ResourceGetRouteName.Get(), new { id })}{s}");
        }

        private HateoasLink ResourceCreateLinkBuilder() =>
            new HateoasLink(
                "create",
                "POST",
                $"{Url.Link(HateoasConfig.ResourceCreateRouteName.Get(), null)}");

        private HateoasLink ResourceUpsertLinkBuilder(Guid id) =>
            new HateoasLink(
                "update",
                "PUT",
                $"{Url.Link(HateoasConfig.ResourceUpsertRouteName.Get(), new { id })}");

        private HateoasLink ResourcePatchLinkBuilder(Guid id) =>
            new HateoasLink(
                "patch",
                "PATCH",
                $"{Url.Link(HateoasConfig.ResourcePatchRouteName.Get(), new { id })}");

        private HateoasLink ResourceDeleteLinkBuilder(Guid id) =>
            new HateoasLink(
                "delete",
                "DELETE",
                $"{Url.Link(HateoasConfig.ResourceDeleteRouteName.Get(), new { id })}");

        private static void AddCustomLinks(IEnumerable<HateoasLink> additionalLinks, List<HateoasLink> links)
        {
            var hateoasLinks = additionalLinks?.ToArray() ?? new List<HateoasLink>().ToArray();
            if (hateoasLinks.Any())
            {
                links.AddRange(hateoasLinks);
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

        private void AddHateoasLinksToResources<TParameters>(IEnumerable<ExpandoObject> resources,
            TParameters usedParameters, IEnumerable<HateoasLink> additionalIndividualLinks)
            where TParameters : CommonResourceParameters
        {
            var individualLinks = additionalIndividualLinks?.ToList() ?? new List<HateoasLink>();

            foreach (IDictionary<string, object> resource in resources)
            {
                // NOTE: Hateoas "_links" support is only available if the ID is available, if
                // the data has been reshaped to not include the Id, no links will be added.
                if (resource.TryGetValue("Id", out var idObj))
                {
                    var links = ResourceGetLinks(
                        new Guid(idObj.ToString()), 
                        usedParameters.Shape, 
                        individualLinks);

                    if (links.Any())
                    {
                        resource.Add("_links", links);
                    }
                }

            }
        }
    }
}