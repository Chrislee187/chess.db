using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public static class StartupExtensions
    {
        public static void RestConfig(this IServiceCollection services)
        {
            services
                .AddControllers(cfg =>
                {
                    cfg.ReturnHttpNotAcceptable = true;     // NOTE: Configures to return 406 for unsupported "Accept" header content-types
                })
                .AddJsonOptions(cfg =>
                {
                    cfg.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .AddNewtonsoftJson()                        // NOTE: Needed for JsonPatchDocument support
                .AddXmlDataContractSerializerFormatters()   // NOTE: Add "application/xml" content-type support
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = new InvalidModelStateResponse().SetupInvalidModelStateResponse;
                }
            );

        }

        public static void ConfigureExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(cfg =>
                {
                    cfg.Run(async context =>
                    {
                        // TODO: Better exception handling and logging
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Please try again.");
                    });
                });
            }
        }
    }
}