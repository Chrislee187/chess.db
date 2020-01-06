using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace chess.db.webapi.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly GlobalExceptionOptions _options;

        public GlobalExceptionHandler(
            GlobalExceptionOptions options, 
            RequestDelegate next,
            ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _options);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, GlobalExceptionOptions opts)
        {
            var error = new ApiError
            {
                Id = Guid.NewGuid().ToString(),
                Status = (short)HttpStatusCode.InternalServerError,
                Code = "",
                Title = "Some kind of error occurred in the API.  Please use the error Id and contact our " +
                        "support team if the problem persists.",
                Detail = ""
            };

            opts.AddResponseDetails?.Invoke(context, exception, error);

            var innerException = GetInnermostException(exception);

            _logger.LogError(exception, innerException.Message + "|{httpStatusCode}|{errorId}", error.Status, error.Id);
            
            var result = JsonConvert.SerializeObject(error);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }

        private Exception GetInnermostException(Exception exception)
        {
            if (exception.InnerException != null)
                return GetInnermostException(exception.InnerException);

            return exception;
        }
    }
}