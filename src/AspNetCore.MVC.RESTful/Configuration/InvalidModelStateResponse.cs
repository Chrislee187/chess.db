using System;
using AspNetCore.MVC.RESTful.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public class InvalidModelStateResponse
    {
        private readonly string _title;
        private readonly int _statusCode;
        private readonly string _details;
        private readonly string _contentType;

        public InvalidModelStateResponse(
            string title = "One or more model validation errors occurred.",
            int statusCode = StatusCodes.Status422UnprocessableEntity,
            string details = "See the errors property for details.",
            string contentType = "application/problem+json"
        )
        {
            _contentType = contentType;
            _details = details;
            _statusCode = statusCode;
            _title = title;
        }
        public IActionResult SetupInvalidModelStateResponse(ActionContext context)
        {
            context = NullX.Throw(context, nameof(context));

            // NOTE: Gives invalid model errors correct status and better details
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Title = _title,
                Status = _statusCode,
                Detail = _details,
                Instance = context.HttpContext.Request.Path
            };

            problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

            return new UnprocessableEntityObjectResult(problemDetails)
            {
                ContentTypes = { _contentType }
            };
        }
    }
}