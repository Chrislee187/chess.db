using System;
using AspNetCore.MVC.RESTful.Filters;
using AspNetCore.MVC.RESTful.Services;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace AspNetCore.MVC.RESTful.Configuration
{
    public static class StartupExtensions
    {
        private const string CorsPolicy = "RESTful-policy";
        /// <summary>
        /// Setup Restful for the application, calls
        /// <code>
        /// 
        /// app.UseRouting();
        ///
        /// app.UseAuthorization();
        ///
        /// app.UseEndpoints(endpoints =>
        /// {
        ///     endpoints.MapControllers();
        /// });
        /// </code>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public static void UseRestful(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            // NOTE: Order is SPECIFIC!
            // i.e. Authorisation `UseAuthorization()` comes after a Route endpoint is chosen `UseRouting()` but before
            // the endpoint is actually executed `UseEndPoints()`


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChessDB API V1");
            });

            app.UseRouting();
            app.UseCors(CorsPolicy);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Configures
        /// <list type="bullet">
        ///     <item> 
        ///         AddControllers(), AddXmlDataContractSerializerFormatters(). 
        ///     </item>
        ///     <item>
        ///         Configures 406 to be returned for unacceptable Content-types
        ///         <code>
        ///             i.e.
        ///             cfg.ReturnHttpNotAcceptable = true;
        ///         </code>
        ///     </item>
        ///     <item> 
        ///         AddNewtonsoftJson() to not return properties with null value and
        ///         is required to use <see cref="JsonPatchDocument{TModel}"/>.
        ///         <code>
        ///             i.e.
        ///             cfg.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        ///         </code>
        ///     </item>
        ///     <item>
        ///         Configures <see cref="ApiBehaviorOptions.InvalidModelStateResponseFactory"/> to use
        ///         <see cref="InvalidModelStateResponse"/>
        ///     </item>
        ///     <item>
        ///         Adds AutoMapper using <see cref="AppDomain.GetAssemblies"/> from the <see cref="AppDomain.CurrentDomain"/>.
        ///         <code>
        ///             i.e.
        ///             services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
        ///         </code>
        ///     </item>
        ///     <item>
        ///         Adds a default <see cref="IOrderByPropertyMappingService{TDto,TEntity}"/> from the <see cref="AppDomain.CurrentDomain"/>
        ///         that assume resource and property names match.
        ///     </item>
        ///     <item>
        ///         Adds Action Filters to support pagination (<see cref="SupportCollectionParamsActionFilter"/>),
        ///         Data shaping (<see cref="SupportDataShapingParamsActionFilter"/>) and
        ///         Hateoas link disabling (<see cref="DisableHateoasLinksActionFilter"/>.
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="services"></param>
        public static void AddRestful(this IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChessDB API", Version = "v1" });
            });

            services
                .AddControllers(cfg =>
                {
                    cfg.ReturnHttpNotAcceptable = true;     // NOTE: Configures to return 406 for unsupported "Accept" header content-types
                })
                .AddNewtonsoftJson(
                    )  // NOTE: Newtonsoft needed for JsonPatchDocument support otherwise would use System.Text.Json
                // NOTE: system.text.json doesn't support JsonPatchDocument yet so use NewtonSoft through out
                // to make sure it's clear whats doing the serialisation.
                // .AddJsonOptions()
                .AddXmlDataContractSerializerFormatters()   // NOTE: Adds "application/xml" content-type support
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    // NOTE: Setup custom response for model (typically from query params etc.) validation errors
                    setupAction.InvalidModelStateResponseFactory 
                        = new InvalidModelStateResponse().SetupInvalidModelStateResponse;
                })
                ;

            services.AddCors(opts =>
            {
                opts.AddPolicy(CorsPolicy, builder =>
                {
                    builder
                        .WithOrigins("http://localhost:4200")
                        .WithExposedHeaders("X-Pagination")
                        ;
                });
            });

            services.AddMvc(opts =>
            {
                opts.Filters.Add<DisableHateoasLinksActionFilter>();

                // NOTE: Add support for collection and data-shaping to RESTful resource endpoints
                // (only for Controllers that inherit from ResourceControllerBase<,>
                opts.Filters.Add<SupportCollectionParamsActionFilter>();
                opts.Filters.Add<SupportDataShapingParamsActionFilter>();
                opts.Filters.Add<PerformanceActionFilter>();
            });

            services.AddTransient(typeof(IOrderByPropertyMappingService<,>), typeof(OrderByPropertyMappingService<,>));
            
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()) // Registers Mapping "Profiles"
                
                ;
        }

        /// <summary>
        /// Configure non development mode exception handling to expose no information.
        /// </summary>
        /// <param name="app">Value from Startup</param>
        /// <param name="env">Value from Startup</param>
        public static void RestfulExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler(cfg =>
                {
                    cfg.Run(async context =>
                    {
                        // TODO: Better exception handling and logging
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Please try again.").ConfigureAwait(false);
                    });
                });
            }
        }
    }
}