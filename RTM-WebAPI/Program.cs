using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RadonTestsManager.DBContext;
using RadonTestsManager.Models;
using RTM;

namespace RadonTestsManager {
    public class Program {
        public static void Main(string[] args) {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                var context = services.GetService<RadonTestsManagerContext>();
                var roleManager = services.GetService<RoleManager<IdentityRole>>();
                var userManager = services.GetService<UserManager<User>>();
                var signInManager = services.GetService<SignInManager<User>>();
                var config = services.GetService<IConfiguration>();
                try {
                    context.Database.Migrate();
                    context.SeedRolesAsync(roleManager).Wait();
                    //TODO next two lines will populate a blank database for testing - uses Bogus
                    context.SeedFakeUsersAsync(userManager, signInManager, config).Wait();
                    context.SeedDevDataAsync().Wait();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured while seeding and migrating the GiveNTake Database.");
                    throw;
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>

            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                })
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5555");
    }
}
