using System.Text.Encodings.Web;
using System.Text.Json;
using chess.games.db.api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace chess.db.webapi.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        public void AddMetadataHeader<T>(PagedList<T> data, IResourceUris urls)
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