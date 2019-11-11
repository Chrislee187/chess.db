using System;
using System.Diagnostics.CodeAnalysis;
using AspNetCore.MVC.RESTful.AutoMapper;
using AspNetCore.MVC.RESTful.Parameters;
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
        
        public static void CheckRestfulMappingsFor<TEntity>(
            [NotNull] this IApplicationBuilder app,
            RestfulEndpointMapping endpoints = RestfulEndpointMapping.Readwrite)
        {
            if (endpoints == RestfulEndpointMapping.Readonly)
            {
                GetChecker(app).CheckReadonly<TEntity>();
            }
            else
            {
                GetChecker(app).Check<TEntity>();
            }
        }

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
                opts.Filters.Add(new EnableHateoasLinksActionFilter());
                // NOTE: PaginationParameter support enabled using Attribute form on individual actions
                // Can be enabled globally using the ActionFilter form here
                // opts.Filters.Add(new SupportsCollectionParams()); 
            });

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
                        await context.Response.WriteAsync("An unexpected fault happened. Please try again.").ConfigureAwait(false);
                    });
                });
            }
        }

        private static RestfulAutoMapperConventionsChecker GetChecker(IApplicationBuilder app) 
            => _mapperChecker ??= new RestfulAutoMapperConventionsChecker(
                    app.ApplicationServices.GetService<IMapper>()
            );
    }
}