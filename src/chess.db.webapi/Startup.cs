using System;
using System.Collections.Generic;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;
using chess.db.webapi.Helpers;
using chess.db.webapi.Middleware;
using chess.db.webapi.Models;
using chess.games.db.api;
using chess.games.db.Configuration;
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


            var serverType = Configuration.ServerType();
            var connectionString = Configuration.ConnectionString();

            services
                .AddChessDatabaseContext(serverType, connectionString)
                .AddChessRepositories();

            services
                .AddTransient<IOrderByPropertyMappingService<GameDto, Game>>(s =>
                        new OrderByPropertyMappingService<GameDto, Game>(
                            new Dictionary<string, OrderByPropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                            {
                                { "White", new OrderByPropertyMappingValue(new List<string>() { $"{nameof(Game.White)}.{nameof(Player.LastName)}"})} ,
                                { "Black", new OrderByPropertyMappingValue(new List<string>() { $"{nameof(Game.Black)}.{nameof(Player.LastName)}"})},
                                { "Event", new OrderByPropertyMappingValue(new List<string>() { $"{nameof(Game.Event)}.{nameof(Event.Name)}" } ) },
                                { "Site", new OrderByPropertyMappingValue(new List<string>() { $"{nameof(Game.Site)}.{nameof(Site.Name)}" } ) },
                                { "Moves", new OrderByPropertyMappingValue(new List<string>() { nameof(Game.MoveText) } ) }
                            })
                    )
                .AddTransient(typeof(IEntityUpdater<,>), typeof(EntityUpdater<,>))
                ;
                    
       }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.RestfulExceptionHandling(env);
            app.UseGlobalExceptionHandler(options =>
            {
//                options.AddResponseDetails = UpdateApiErrorResponse;
            });

            app.UseRestful(env);

        }
    }
}
