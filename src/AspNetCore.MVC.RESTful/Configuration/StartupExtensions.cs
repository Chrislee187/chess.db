using System;
using AspNetCore.MVC.RESTful.Parameters;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public static class StartupExtensions
    {
        public static void UseRestful(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            // NOTE: Order is SPECIFIC!
            // i.e. Authorisation `UseAuthorization()` comes after a Route endpoint is chosen `UseRouting()` but before
            // the endpoint is actually executed `UseEndPoints()`

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        public static void AddRestful(this IServiceCollection services)
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

            services.AddTransient(typeof(IOrderByPropertyMappingService<,>), typeof(OrderByPropertyMappingService<,>));
            
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()) // Registers Mapping "Profiles"
                
                ;
        }

        public static void RestfulExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment env)
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