using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AspNetCore.MVC.RESTful.Filters
{
    public class PerformanceActionFilter : IActionFilter
    {
        private Stopwatch _timer;
        private readonly ILogger<PerformanceActionFilter> _logger;

        public PerformanceActionFilter(ILogger<PerformanceActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _timer.Stop();
            if (context.Exception == null)
            {
                _logger.LogRoutePerformance(context.HttpContext.Request.Path,
                    context.HttpContext.Request.Method,
                    _timer.ElapsedMilliseconds);
            }
        }
    }
}