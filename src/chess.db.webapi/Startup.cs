using System;
using System.Collections.Generic;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Parameters;
using chess.db.webapi.Helpers;
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
                .AddRestful();

            services
                .AddChessDatabaseContext(Configuration["ChessDB"])
                .AddChessRepositories();

            services
                .AddTransient<IOrderByPropertyMappingService<PlayerDto, Player>>(s =>
                        new OrderByPropertyMappingService<PlayerDto, Player>(
                            new Dictionary<string, OrderByPropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                            {
                                { "Lastname", new OrderByPropertyMappingValue(new List<string>() { "Surname" } ) }
                            })
                    )
                .AddTransient(typeof(IEntityUpdater<>), typeof(EntityUpdater<>))
                ;
                    
       }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.RestfulExceptionHandling(env);

            app.UseRestful(env);

            app.CheckRestfulMappingsFor<Player>(RestfulEndpointMapping.Readwrite);
            app.CheckRestfulMappingsFor<PgnPlayer>(RestfulEndpointMapping.Readonly);
        }
    }
}
