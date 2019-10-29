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

        public void AddPaginationHeader<T>(PagedList<T> data
        , string previous, string next)
        {
            var paginationMetadata = new
            {
                totalCount = data.TotalCount,
                pageSize = data.PageSize,
                currentPage = data.CurrentPage,
                totalPages = data.TotalPages,
                previousPageLink = previous,
                nextPageLink = next
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        }
    }
}