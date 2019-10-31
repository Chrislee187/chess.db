using System;
using System.Collections.Generic;
using AspNetCore.MVC.RESTful.AutoMapper;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Parameters;
using AutoMapper;
using chess.db.webapi.Models;
using chess.games.db;
using chess.games.db.api;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace chess.db.webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .RestConfig();

            services
                    .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()) // Registers Mapping "Profiles"
                    .AddChessDatabaseContext(Configuration["ChessDB"])
                    .AddChessRepositories()
                    .AddTransient<IOrderByPropertyMappingService<PlayerDto, Player>>(s =>
                        new OrderByPropertyMappingService<PlayerDto, Player>(
                            new Dictionary<string, OrderByPropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                            {
                                { "Lastname", new OrderByPropertyMappingValue(new List<string>() { "Surname" } ) }
                            })
                    )
                    .AddTransient<IOrderByPropertyMappingService<PgnPlayerDto, PgnPlayer>>(s =>
                        new OrderByPropertyMappingService<PgnPlayerDto,PgnPlayer>()
                    );
       }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureExceptionHandling(env);

            // NOTE: Order is SPECIFIC!
            // i.e. Authorisation `UseAuthorization()` comes after a Route endpoint is chosen `UseRouting()` but before
            // the endpoint is actually executed `UseEndPoints()`

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            CheckAutoMapperConventionsForRestful(app);
        }

        private static void CheckAutoMapperConventionsForRestful(IApplicationBuilder app)
        {
            var mapper = app.ApplicationServices.GetService<IMapper>();
            new AutoMapperConventionsChecker(mapper).Check<Player>();
            new AutoMapperConventionsChecker(mapper).CheckReadonly<PgnPlayer>();
        }
    }
}
