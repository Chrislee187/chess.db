using System;
using System.Diagnostics.CodeAnalysis;
using AspNetCore.MVC.RESTful.AutoMapper;
using AspNetCore.MVC.RESTful.Filters;
using AspNetCore.MVC.RESTful.Services;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public static class StartupExtensions
    {
        private static RestfulAutoMapperConventionsChecker _mapperChecker;

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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Add Controllers, NewtonsoftJson, Xml Data Contract Formatters. 
        /// Configures InvalidModelStateResponseFactory. 
        /// Adds AutoMapper using <see cref="AppDomain.GetAssemblies"/> from the <see cref="AppDomain.CurrentDomain"/>.
        /// Configures 406 to be returned for unacceptable Content-types
        /// Set Null value handling to ignore nulls when serializing output
        /// </summary>
        /// <param name="services"></param>
        public static void AddRestful(this IServiceCollection services)
        {
            services
                .AddControllers(cfg =>
                {
                    cfg.ReturnHttpNotAcceptable = true;     // NOTE: Configures to return 406 for unsupported "Accept" header content-types
                })
                .AddNewtonsoftJson(cfg =>
                {
                    // NOTE: Newtonsoft needed for JsonPatchDocument support otherwise would use System.Text.Json below
                    cfg.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }) 
                // NOTE: system.text.json doesn't support JsonPatchDocument yet so use NewtonSoft through out
                // to make sure it's clear whats doing the serialisation.
//                .AddJsonOptions(cfg =>
//                {
//                    cfg.JsonSerializerOptions.IgnoreNullValues = true;
//                })
                .AddXmlDataContractSerializerFormatters()   // NOTE: Adds "application/xml" content-type support
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    // NOTE: Setup custom response for model (typically from query params etc.) validation errors
                    setupAction.InvalidModelStateResponseFactory = new InvalidModelStateResponse().SetupInvalidModelStateResponse;
                }
            );

            services.AddMvc(opts =>
            {
                opts.Filters.Add(new DisableHateoasLinksActionFilter());

                // NOTE: Add support for collection and data-shaping to RESTful resource endpoints
                // (only for Controllers that inherit from ResourceControllerBase<,>
                opts.Filters.Add(new SupportCollectionParamsActionFilter());
                opts.Filters.Add(new SupportDataShapingParamsActionFilter());
            });

            services.AddTransient(typeof(IOrderByPropertyMappingService<,>), typeof(OrderByPropertyMappingService<,>));
            
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()) // Registers Mapping "Profiles"
                
                ;
        }

        /// <summary>
        /// Configure non development mode exception handling to expose no information.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
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

        private static RestfulAutoMapperConventionsChecker GetChecker(IApplicationBuilder app) 
            => _mapperChecker ??= new RestfulAutoMapperConventionsChecker(
                    app.ApplicationServices.GetService<IMapper>()
            );

        public static void CheckRestfulMappingsFor<TEntity>(
            [NotNull] this IApplicationBuilder app,
            RestfulEndpointMappingChecks endpoints = RestfulEndpointMappingChecks.Readwrite)
        {
            if (endpoints == RestfulEndpointMappingChecks.Readonly)
            {
                GetChecker(app).CheckReadonly<TEntity>();
            }
            else
            {
                GetChecker(app).Check<TEntity>();
            }
        }

        public static void CheckRestfulMappingsFor<TEntity>(
            [NotNull] this IApplicationBuilder app,
            bool checkResourceGet = true,
            bool checkResourcesGet = true,
            bool checkResourceCreate = true,
            bool checkResourceUpdate = true)
        {
            GetChecker(app).Check<TEntity>(
                checkResourceGet, 
                checkResourcesGet, 
                checkResourceCreate, 
                checkResourceUpdate
            );
        }
    }
}