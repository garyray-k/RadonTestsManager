using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadonTestsManager.DBContext;

namespace RadonTestsManager {
    public class Startup {
// This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            var sqlPass = System.Environment.GetEnvironmentVariable("SQL-PASSWORD");
            services.AddDbContext<RadonTestsManagerContext>(options =>
                options.UseSqlServer($"Server=127.0.0.1,1433;Database=GiveNTake;User Id=sa;Password={sqlPass};Trusted_Connection=false;MultipleActiveResultSets=True"));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseDefaultFiles();

            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".ts"] = "application/x-typescript";
            StaticFileOptions staticFileOptions = new StaticFileOptions() {
                ContentTypeProvider = provider
            };

            app.UseStaticFiles(staticFileOptions);

            app.UseMvc(routes => {
                routes
                    .MapRoute( name: "default", template: "{controller=Home}/{action=Index}/{id?}")
                    .MapRoute(
                        name: "api", 
                        template: "api/{controller}/{action}/{id?}", 
                        defaults: new { Controller = "Messages", action = "My" },
                        constraints: new { id = new IntRouteConstraint() });
                routes
                    .MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            app.Run(async (context) => {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
