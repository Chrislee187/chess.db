using System;
using Microsoft.AspNetCore.Builder;

namespace chess.db.webapi.Middleware
{
    public static class GlobalExceptionExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            var options = new GlobalExceptionOptions();
            return builder.UseMiddleware<GlobalExceptionHandler>(options);
        }

        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder,
            Action<GlobalExceptionOptions> configureOptions)
        {
            var options = new GlobalExceptionOptions();
            configureOptions(options);

            return builder.UseMiddleware<GlobalExceptionHandler>(options);
        }
    }
}