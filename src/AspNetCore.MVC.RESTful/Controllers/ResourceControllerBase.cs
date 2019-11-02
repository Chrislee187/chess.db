using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using AspNetCore.MVC.RESTful.AutoMapper;
using AspNetCore.MVC.RESTful.Helpers;
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
    /// A base class for an MVC Controller that supports AddRestful endpoints.
    /// Relies heavily on Automapper mapping abilities to Generic'ify the process for simple reuse
    /// <seealso cref="RestfulAutoMapperConventionsChecker"></seealso>
    /// </summary>
    /// <remarks>
    ///     JSON/XML content-types
    ///     correct status-code usage
    ///     standardised and detailed error responses
    ///
    /// </remarks>
    /// <typeparam name="TDto">Data Transfer Object that can be Automapped from TEntity</typeparam>
    /// <typeparam name="TEntity">Underlying Entity for the Resource being represented</typeparam>
    public abstract class ResourceControllerBase<TDto, TEntity> : ControllerBase 
        where TEntity : class
                where TDto : IResourceId
    {
        private readonly IResourceRepository<TEntity> _restResourceRepository;
        private readonly IOrderByPropertyMappingService<TDto, TEntity> _orderByPropertyMappingService;
        private readonly IEntityUpdater<TEntity> _entityUpdater;

        protected ResourceControllerBase(IMapper mapper,
            IResourceRepository<TEntity> resourceRepository,
            IOrderByPropertyMappingService<TDto, TEntity> orderByPropertyMappingService,
            IEntityUpdater<TEntity> entityUpdater)
        {
            Mapper = NullX.Throw(mapper, nameof(mapper));
            _restResourceRepository = NullX.Throw(resourceRepository, nameof(resourceRepository));
            _orderByPropertyMappingService = NullX.Throw(orderByPropertyMappingService, nameof(orderByPropertyMappingService));
            _entityUpdater = NullX.Throw(entityUpdater, nameof(entityUpdater));
        }

        protected IMapper Mapper { get; }

        /// <summary>
        /// HTTP GET /{resource}
        /// </summary>
        protected ActionResult<IEnumerable<TDto>> ResourcesGet<TParameters>(
            TParameters parameters,
            IResourceQuery<TEntity> filters,
            IResourceQuery<TEntity> resourceQuery, 
            string resourcesGetRouteName) 
                where TParameters : CommonResourceParameters
        {
            string LinkBuilder(object queryStringParams)
                => Url.Link(resourcesGetRouteName, queryStringParams);

            var orderBy = Mapper.Map<OrderByParameters>(parameters);
            
            var orderByCheck = _orderByPropertyMappingService
                .ClauseIsValid(orderBy.Clause);

            if (!orderByCheck.Valid)
            {
                orderByCheck.Details.Instance = Url.Link(resourcesGetRouteName, parameters);
                return BadRequest(orderByCheck.Details);
            }

            var pagination = Mapper.Map<PaginationParameters>(parameters);

            var orderByMappings = _orderByPropertyMappingService.GetPropertyMapping();

            var resources = _restResourceRepository.Load(filters, resourceQuery, pagination, orderBy, orderByMappings);

            var usedParameters = Activator.CreateInstance<TParameters>();

            Mapper.Map(pagination, usedParameters);
            Mapper.Map(orderBy, usedParameters);
            Mapper.Map(filters, usedParameters);
            Mapper.Map(resourceQuery, usedParameters);

            AddPaginationMetadataHeader(
                resources,
                new ResourceUriBuilder(
                    resources,
                    usedParameters,
                    LinkBuilder
                )
            );

            return Ok(Mapper.Map<IEnumerable<TDto>>(resources));
        }

        /// <summary>
        /// HTTP GET /{resource}/{id}
        /// </summary>
        protected ActionResult<TDto> ResourceGet(Guid id)
        {
            var resource = _restResourceRepository.Load(id);

            if (resource == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<TDto>(resource));
        }

        /// <summary>
        /// HTTP POST
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

            // NOTE: CreatedAtRoute returns 201 (Created) and places a 'Location' entry in
            // the response header containing the uri to retrieve the newly added resource
            return CreatedAtRoute(
                resourceGetRouteName,
                new { createdResource.Id },
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
                    new { id },
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
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
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

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata, new JsonSerializerOptions()
            {
                // NOTE: Stops the '?' & '&' chars in the links being escaped
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

        }
    }
}