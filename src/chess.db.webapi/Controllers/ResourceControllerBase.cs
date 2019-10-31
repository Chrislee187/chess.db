using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using AutoMapper;
using chess.db.webapi.ResourceParameters;
using chess.db.webapi.Services;
using chess.games.db.api;
using chess.games.db.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace chess.db.webapi.Controllers
{
    /// <summary>
    /// Base class support to allow a controller to easily support standard REST actions
    /// that support
    ///     JSON/XML content-types
    ///     correct status-code usage
    ///     standardised and detailed error responses
    ///
    /// Relies heavily on Automapper mapping abilities to Generic'ify the process for simple reuse
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ResourceControllerBase<TDto, TEntity> : ControllerBase 
        where TEntity : class, IDbEntity
    {
        protected readonly IMapper Mapper;
        private readonly IResourceRepositoryBase<TEntity> _restResourceRepository;
        private readonly IOrderByPropertyMappingService _orderByPropertyMappingService;

        protected ResourceControllerBase(
                IMapper mapper,
                IResourceRepositoryBase<TEntity> resourceRepository,
                IOrderByPropertyMappingService orderByPropertyMappingService
            )
        {
            _orderByPropertyMappingService = orderByPropertyMappingService ?? throw new ArgumentNullException(nameof(orderByPropertyMappingService));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _restResourceRepository = resourceRepository ?? throw new ArgumentNullException(nameof(resourceRepository));
        }

        /// <summary>
        /// HTTP GET /{resource}
        /// </summary>
        protected ActionResult<IEnumerable<TDto>> ResourcesGet<TParameters>(
            TParameters parameters,
            Query<TEntity> filters,
            Query<TEntity> query, 
            string resourcesGetRouteName) 
                where TParameters : CommonResourceParameters
        {
            string LinkBuilder(object queryStringParams)
                => Url.Link(resourcesGetRouteName, queryStringParams);

            var orderBy = Mapper.Map<OrderByParameters>(parameters);

            var orderByCheck = _orderByPropertyMappingService
                .ClauseIsValid<TDto, TEntity>(orderBy.Clause);

            if (!orderByCheck.Valid)
            {
                orderByCheck.Details.Instance = Url.Link(resourcesGetRouteName, parameters);
                return BadRequest(orderByCheck.Details);
            }

            var pagination = Mapper.Map<PaginationParameters>(parameters);

            var orderByMappings = _orderByPropertyMappingService.GetPropertyMapping<TDto, TEntity>();

            var resources = _restResourceRepository.Get(filters, query, pagination, orderBy, orderByMappings);

            var usedParameters = Activator.CreateInstance<TParameters>();

            Mapper.Map(pagination, usedParameters);
            Mapper.Map(orderBy, usedParameters);
            Mapper.Map(filters, usedParameters);
            Mapper.Map(query, usedParameters);

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
            var resource = _restResourceRepository.Get(id);

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
                new { entity.Id },
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

            var entity = _restResourceRepository.Get(id);

            ActionResult result;
            if (entity == null)
            {
                var addedEntity = Mapper.Map<TEntity>(model);
                addedEntity.Id = id;
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
            JsonPatchDocument<TUpdateDto> patchDocument) 
            where TUpdateDto : class
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }

            var resource = _restResourceRepository.Get(id);
            if (resource == null)
            {
                return NotFound();
            }

            var patchedResource = Mapper.Map<TUpdateDto>(resource);
            patchDocument.ApplyTo(patchedResource, ModelState);

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

            var resource = _restResourceRepository.Get(id);

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
        public IActionResult ResourceOptions(params string[] httpMethods)
        {
            Response.Headers.Add("Allow", string.Join(',', httpMethods));
            return Ok();
        }

        /// <summary>
        /// Simple override to use the same error format as the framework for manually
        /// raised validation issues.
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
            PagedList<T> data, 
            IResourceUris urls)
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