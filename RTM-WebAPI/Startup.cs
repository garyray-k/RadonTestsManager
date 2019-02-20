using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RadonTestsManager.DBContext;
using RadonTestsManager.Models;
using RadonTestsManager.Utility;
using Swashbuckle.AspNetCore.Swagger;

namespace RadonTestsManager {
    public class Startup {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration config) {
            Configuration = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            var sqlPass = System.Environment.GetEnvironmentVariable("SQL-PASSWORD");
            services.AddDbContext<RadonTestsManagerContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<RadonTestsManagerContext>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(option => {
                    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions => {
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters() {
                        ValidateActor = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["JWTConfiguration:Issuer"],
                        ValidAudience = Configuration["JWTConfiguration:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTConfiguration:SigningKey"]))
                    };
                });
            services.AddCors();
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                config.Filters.Add<GlobalExceptionFilter>();
            });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "RadonTestsManager.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme {
                    Name = "Authorization",
                    In = "header"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    {"Bearer", new string[] {}}
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            var appInsightLogLevel = Configuration.GetValue<LogLevel>("Logging:Application Insights:LogLevel:Default");
            loggerFactory.AddApplicationInsights(app.ApplicationServices, appInsightLogLevel);

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseDefaultFiles();

            app.UseCorrelationIdHeader();

            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".ts"] = "application/x-typescript";
            StaticFileOptions staticFileOptions = new StaticFileOptions() {
                ContentTypeProvider = provider
            };
            app.UseStaticFiles(staticFileOptions);
            app.UseAuthentication();
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

            app.UseCors(b => {
                b.WithHeaders();
                b.AllowAnyMethod();
                //b.WithOrigins();
                b.AllowAnyOrigin();
            });

            if (env.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RadonTestsManager.API");
                });
            }
            app.Run(async (context) => {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
