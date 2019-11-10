using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public class SupportsPaginationParams : PaginationParamsBase
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.Controller is HateoasController contextController)
            {
                var queryCollection = contextController.Request.Query;

                GetPageSize(queryCollection, contextController);
                GetCurrentPage(queryCollection, contextController);
            }
        }
   }

    public abstract class PaginationParamsBase : ActionFilterAttribute
    {
        protected static void GetPageSize(IQueryCollection queryCollection, HateoasController contextController)
        {
            var pageSizeKeys = new[] { "pagesize", "page-size" };

            var val = queryCollection.ArgValue(pageSizeKeys);

            if (int.TryParse(val, out int pageSize))
            {
                contextController.PaginationParameters.PageSize = pageSize;
            }
        }

        protected static void GetCurrentPage(IQueryCollection queryCollection, HateoasController contextController)
        {
            var currentPageKeys = new[] { "page", "currentpage", "current-page", "page-number" };
            var val = queryCollection.ArgValue(currentPageKeys);

            if (int.TryParse(val, out int page))
            {
                contextController.PaginationParameters.Page = page;
            }
        }

    }
}