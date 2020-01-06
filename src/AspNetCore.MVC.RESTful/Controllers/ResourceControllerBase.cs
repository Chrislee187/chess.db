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
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Filters;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Repositories;
using AspNetCore.MVC.RESTful.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace AspNetCore.MVC.RESTful.Controllers
{
    /// <summary>
    /// Abstract implementation of an MVC Controller that has support for standard "CRUD" Resource operations, namely
    /// <list type="bullet">
    ///     <item><see cref="ResourceGet"/> typically called from <c>HTTP GET</c> actions that return collections/lists of resources</item>
    ///     <item><see cref="ResourcesGet{TParameters}"/> typically called from <c>HTTP GET</c> actions that return individual resources</item>
    ///     <item><see cref="ResourceCreate{TCreationDto}"/> typically called from <c>HTTP POST</c> actions create new resources</item>
    ///     <item><see cref="ResourceDelete"/> typically called from <c>HTTP DELETE</c> actions that delete resources</item>
    ///     <item><see cref="ResourceUpsert{TUpdateDto}"/> typically called from <c>HTTP PUT</c> actions that upsert (update if exists, create otherwise) resources</item>
    ///     <item><see cref="ResourceOptions"/>
    ///         typically called from a <c>HTTP OPTIONS</c> action and returns the HTTP METHODS by the Controller, an empty resource (for template/informational purposes) and
    ///         some details on the query string parameters. (Authors Note: Couldn't find a standard for handling this kind of discovery, here seemed reasonable)
    ///     </item>
    /// </list>
    /// <para>
    /// Each of these operations has built-in support (where appropriate for the operation) for
    /// pagination, filtering, sorting, searching and data-shaping. Note filtering and searching
    /// currently require small custom implementations per resource.
    /// </para>
    /// <para>
    /// Correctly implemented, calls to Resource methods can be executed in a single line in your controllers.
    /// </para>
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
        where TDto : class, IResourceId<Guid>
    {
        private readonly ILogger<ResourceControllerBase<TDto, TEntity, TId>> _logger;
        private readonly IResourceRepository<TEntity, TId> _restResourceRepository;
        private readonly IOrderByPropertyMappingService<TDto, TEntity> _orderByPropertyMappingService;
        private readonly IEntityUpdater<TEntity, TId> _entityUpdater;

        protected IMapper Mapper { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper">AutoMapper instance</param>
        /// <param name="resourceRepository">Resource repository instance</param>
        /// <param name="entityUpdater">Entity ID Updater required for Upserts </param>
        /// <param name="orderByPropertyMappingService">Optional services to map resource property names to entity property names</param>
        /// <param name="config">Optional configuration overides.</param>
        /// <param name="logger">Optional logger.</param>
        protected ResourceControllerBase([NotNull] IMapper mapper,
            [NotNull] IResourceRepository<TEntity, TId> resourceRepository,
            IEntityUpdater<TEntity, TId> entityUpdater,
            IOrderByPropertyMappingService<TDto, TEntity> orderByPropertyMappingService = null,
            Action<HateoasConfig> config = null,
            ILogger<ResourceControllerBase<TDto, TEntity, TId>> logger = null)
        {
            _logger = logger;
            ConfigureHateoas(config);

            Mapper = NullX.Throw(mapper, nameof(mapper));
            _restResourceRepository = NullX.Throw(resourceRepository, nameof(resourceRepository));
            _orderByPropertyMappingService = NullX.Throw(orderByPropertyMappingService, nameof(orderByPropertyMappingService));
            _entityUpdater = NullX.Throw(entityUpdater, nameof(entityUpdater));

        }

        /// <summary>
        /// Typically used to support a HTTP GET on a resource collection
        /// <code>
        /// HTTP GET /{resources}
        /// </code>
        /// Response is paginated based on values in <see cref="CollectionConfig"/> which are set via
        /// querystring parameters, see <seealso cref="SupportCollectionParamsActionFilter"/>
        /// </summary>
        /// <returns>
        /// <see cref="BadRequestResult"/> if the shape is invalid.
        /// <see cref="BadRequestResult"/> if the order-by clause is invalid.
        ///
        /// other wise
        /// 
        /// <see cref="OkResult"/> with response body containing the serialized representation
        /// of the resources. 
        /// </returns>
        protected IActionResult ResourcesGet()
        {
            return ResourcesGet<object>(null,null,null);
        }

        protected IActionResult ResourcesGet<TParameters>(TParameters parameters,
            IEntityFilter<TEntity> entityFilter,
            IEntitySearch<TEntity> entitySearch) where TParameters : class
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
            
            if (HateoasConfig.AddLinks)
            {
                AddHateoasLinksToResourceCollection(resources);
            }
            
            if (HateoasConfig.AddLinks)
            {
                var collectionLinks = ResourcesGetLinks(parameters, pagedEntities);

                var expandoObject = new {value = resources}.ShapeData("");
                expandoObject.TryAdd(HateoasConfig.LinksPropertyName, collectionLinks);
                
                return Ok(expandoObject);
            }

            return Ok(resources);
        }

        /// <summary>
        /// Typically used to support a HTTP GET on an individual resource within a collection
        /// <code>
        /// HTTP GET /{resources}/{id}
        /// </code>
        /// </summary>
        /// <returns>
        /// <see cref="BadRequestResult"/> if the shape is invalid.
        /// <see cref="NotFoundResult"/> if the resource specified cannot be found.
        ///
        /// other wise
        /// 
        /// <see cref="OkResult"/> with response body containing the serialized representation
        /// of the resource.
        /// </returns>
        protected IActionResult ResourceGet(TId id)
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

            if (HateoasConfig.AddLinks)
            {
                if (resource.ContainsKey("Id"))
                {
                    resource.Add(HateoasConfig.LinksPropertyName, ResourceGetLinks(id, CollectionConfig.Shape));
                }
            }

            return Ok(resource);
        }

        /// <summary>
        /// Typically used to support a HTTP POST on an individual resource within a collection
        /// <code>
        /// HTTP POST /{resources}
        /// {
        ///     ...
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="TCreationDto">Model containing data used for creation, requires an
        /// <see cref="AutoMapper"/> mapping between <typeparamref name="TCreationDto"/>-><typeparamref name="TDto"/></typeparam>
        /// <param name="model">New instance of the resource to create</param>
        /// <returns>
        /// <see cref="CreatedAtRouteResult"/> with the newly created resource in the body.
        /// </returns>
        protected IActionResult ResourceCreate<TCreationDto>(TCreationDto model) 
        {
            var entity = Mapper.Map<TEntity>(model);

            _restResourceRepository.Add(entity);
            _restResourceRepository.Save();

            var createdResource = Mapper.Map<TDto>(entity);

            IDictionary<string, object> resource = createdResource.ShapeData("");
            
            if (HateoasConfig.AddLinks)
            {
                dynamic id = createdResource.Id;
                resource.Add(HateoasConfig.LinksPropertyName, ResourceCreateLinks(id));
            }

            return CreatedAtRoute(
                HateoasConfig.ResourcesGetRouteName,
                new {createdResource.Id},
                createdResource.ShapeData(CollectionConfig.Shape)
            );
        }

        /// <summary>
        /// Typically used to support a HTTP PUT on an individual resource within a collection
        /// <code>
        /// HTTP PUT /{resources}/{id}
        /// </code>
        /// <code>
        /// body contains serialized <typeparamref name="TUpdateDto"/>
        /// </code>
        /// </summary>
        /// <typeparam name="TUpdateDto">Model containing data used for update, requires bi-directional
        /// <see cref="AutoMapper"/> mappings between <typeparamref name="TDto"/> i.e.
        /// <typeparamref name="TUpdateDto"/>-><typeparamref name="TDto"/> and
        /// <typeparamref name="TDto"/>-><typeparamref name="TUpdateDto"/></typeparam>
        /// <param name="id">id of the resource</param>
        /// <param name="model">New instance of the resource to create</param>
        /// <returns>
        /// <see cref="NotFoundResult"/> if the resource specified cannot be found.
        /// <see cref="CreatedAtRouteResult"/> with the newly created resource in the body.
        /// </returns>
        protected ActionResult ResourceUpsert<TUpdateDto>(
            TId id,
            [NotNull] TUpdateDto model)
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

                if (HateoasConfig.AddLinks)
                {
                    resource.Add(HateoasConfig.LinksPropertyName, ResourceUpsertLinks(id));
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
        /// Typically used to support a HTTP PATCH on an individual resource within a collection.
        /// MVC Model Validation is performed using <see cref="ControllerBase.TryValidateModel(object)"/>
        /// and <seealso cref="System.ComponentModel.DataAnnotations"/>
        /// <code>
        /// HTTP PATCH /{resources}/{id}
        /// </code>
        /// <code>
        /// body contains serialized <see cref="JsonPatchDocument{TModel}"/>
        /// </code>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns>
        /// <see cref="NotFoundResult"/> if the resource specified cannot be found.
        /// <see cref="UnprocessableEntityObjectResult"/> containing a serialized <see cref="ValidationProblem"/>
        /// if model validation fails.
        /// 
        /// other wise
        /// 
        /// <see cref="OkResult"/> with response body containing the serialized representation
        /// of the resource.
        /// </returns>
        protected ActionResult ResourcePatch(TId id,
            [NotNull] JsonPatchDocument<TDto> patchDocument)
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

            var patchedResource = Mapper.Map<TDto>(resource);
            patchDocument.ApplyTo(patchedResource);

            if (!TryValidateModel(patchedResource))
            {
                return ValidationProblem(ModelState);
            }

            Mapper.Map(patchedResource, resource);

            _restResourceRepository.Update(resource);
            _restResourceRepository.Save();

            if (HateoasConfig.AddLinks)
            {
                IDictionary<string, object> resourceForLinks = patchedResource.ShapeData("");

                resourceForLinks.Add(HateoasConfig.LinksPropertyName, ResourcePatchLinks(id));
                return Ok(resourceForLinks.ShapeData(CollectionConfig.Shape));
            }

            return NoContent();
        }

        /// <summary>
        /// Typically used to support a HTTP DELETE on an individual resource within a collection.
        /// <code>
        /// HTTP DELETE /{resources}/{id}
        /// </code>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// 
        /// <see cref="NotFoundResult"/> if the resource specified cannot be found.
        ///
        /// otherwise returns
        /// 
        /// <see cref="NoContentResult"/>
        /// </returns>
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
        /// Typically used to support a HTTP OPTIONS call on an resource collection.
        /// <code>
        /// HTTP OPTIONS
        /// </code>
        /// 
        /// Automatically generates a list of supported options by reflecting on the <see cref="HttpMethodAttribute"/>'s
        /// that adorn the actions of the implementing Controller class.
        /// 
        /// </summary>
        /// <returns>
        /// <see cref="OkResult"/>With an empty resource represented by <typeparamref name="TDto"/>
        /// that can be used as a template for other requests
        /// </returns>
        [HttpOptions]
        public IActionResult ResourceOptions()
        {
            Response.Headers.Add("Allow", string.Join(',', HttpOptions));

            var response = new
            {
                query_string_parameters = new []
                {
                    "?page=x&page-size=y                                                // Resource Collections Gets",
                    "?order-by=resource_property_name1, resource_property_name2...      // Resource Collections Gets",
                    "?shape=resource_property_name1, resource_property_name2...         // All calls with resource outputs",
                    "?search=string                                                     // Resource Collections Gets",
                    "?nolinks                                                           // All"
                }
            };

            return Ok(response);
        }
        
        /// <summary>
        /// Creates an <see cref="ActionResult"></see> generated by the default invalid model state response handler.
        /// </summary>
        /// <param name="modelStateDictionary">Model State to validate</param>
        /// <returns>
        /// <see cref="UnprocessableEntityResult"/> containing a serialized <see cref="ProblemDetails"/>
        /// which in turn contains the validation errors.
        /// </returns>
        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext?.RequestServices?
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();

            if (options == null)
            {
                return (ActionResult) new InvalidModelStateResponse().SetupInvalidModelStateResponse(ControllerContext);
            }


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
            Response.Headers.Add(xPaginationHeader.Value);
        }
    }
}