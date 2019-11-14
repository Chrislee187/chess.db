using System;
using System.Collections.Generic;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;
using chess.db.webapi.Helpers;
using chess.db.webapi.Models;
using chess.games.db;
using chess.games.db.api;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                                { nameof(PlayerDto.Lastname), new OrderByPropertyMappingValue(new List<string>() { nameof(Player.Surname) } ) }
                            })
                    )
                .AddTransient(typeof(IEntityUpdater<,>), typeof(EntityUpdater<,>))
                ;
                    
       }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.RestfulExceptionHandling(env);
            
            app.UseRestful(env);

            // TODO: We could automate this further by finding all types in assembly that inherit from
            // ResourceControllerBase, determine there generic types and automatically call CheckRestfulMappingsFor<TEntity>
            // How to cater for RW/RO ?????
            app.CheckRestfulMappingsFor<Player>(RestfulEndpointMappingChecks.Readwrite);
            app.CheckRestfulMappingsFor<PgnPlayer>(RestfulEndpointMappingChecks.Readonly);
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
