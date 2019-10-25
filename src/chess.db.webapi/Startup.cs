using chess.games.db.api;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
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
                .AddControllers(cfg =>
                {
                    cfg.ReturnHttpNotAcceptable = true; // NOTE: Configures to return 406 for unsupported "Accept" header content-types
                })
                .AddXmlDataContractSerializerFormatters(); // NOTE: Add "application/xml" content-type support

            services.AddScoped<IPlayersRepository, PlayersRepository>();

            services.AddDbContext<ChessGamesDbContext>(opts =>
                opts.UseSqlServer(@"Server=.\Dev;Database=ChessGames;Trusted_Connection=True;")
                );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // NOTE: Order is SPECIFIC!
            // i.e. Authorisation comes after a Route endpoint is chosen (UseRouting) but before
            // the endpoint is actually executed (UseEndPoints)

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
