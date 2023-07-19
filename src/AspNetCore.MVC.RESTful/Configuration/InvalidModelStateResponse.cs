using AspNetCore.MVC.RESTful.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.RESTful.Configuration
{
    /// <summary>
    /// Response factory used to configure MVC Api behaviors to produce invalid model state responses
    /// that use the proposed standard <see cref="ProblemDetails"/> response format.
    /// </summary>
    public class InvalidModelStateResponse
    {
        private readonly string _title;
        private readonly int _statusCode;
        private readonly string _details;
        private readonly string _contentType;
        private readonly string _traceIdKey;
        private readonly string _problemType;

        public InvalidModelStateResponse(
            string title = "One or more model validation errors occurred.",
            int statusCode = StatusCodes.Status422UnprocessableEntity,
            string details = "See the errors property for details.",
            string problemType = "https://example.com/invalid-model-state",
            string contentType = "application/problem+json",
            string traceIdKey = "trace-id"
        )
        {
            _problemType = problemType;
            _title = title;
            _statusCode = statusCode;
            _details = details;
            _contentType = contentType;
            _traceIdKey = traceIdKey;
        }
        public IActionResult SetupInvalidModelStateResponse(ActionContext context)
        {
            context = NullX.Throw(context, nameof(context));

            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Title = _title,
                Status = _statusCode,
                Detail = _details,
                Instance = context.HttpContext.Request.Path,
                Type = _problemType
            };

            problemDetails.Extensions.Add(_traceIdKey, context.HttpContext.TraceIdentifier);

            return new UnprocessableEntityObjectResult(problemDetails)
            {
                ContentTypes = { _contentType }
            };
        }
    }
}