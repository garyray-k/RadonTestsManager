using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RadonTestsManager.Controllers;
using RadonTestsManager.DTOs;
using RadonTestsManager.Models;
using RTM.Server.Utility;

namespace RadonTestsManager.DBContext {
    public class RadonTestsManagerContext : IdentityDbContext<User> {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<ContinuousRadonMonitor> ContinuousRadonMonitors { get; set; }
        public DbSet<LSVial> LSVials { get; set; }
        public DbSet<CRMMaintenanceLogEntry> CRMMaintenanceLogs { get; set; }

        public RadonTestsManagerContext(DbContextOptions<RadonTestsManagerContext> options) : base(options) {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            // a very ugly way to avoid 'SetNull' on Identity User/Role but do it for other FKs
            var fks = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            var identityStrings = new List<string>() { "Key: IdentityRole.Id PK", "Key: User.Id PK" };
            foreach (var relationship in fks) {
                if (identityStrings.Contains(relationship.PrincipalKey.ToString())) continue;
                relationship.DeleteBehavior = DeleteBehavior.SetNull;
            }

            //modelBuilder.Entity<ContinuousRadonMonitor>()
                //.HasMany<Job>()
                //.WithOne()
                //.OnDelete(DeleteBehavior.SetNull);
        }

        public async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager) {
            if (!await roleManager.RoleExistsAsync("Admin")) {
                var admin = new IdentityRole("Admin");
                await roleManager.CreateAsync(admin);
            }
            if (!await roleManager.RoleExistsAsync("OfficeStaff")) {
                var officeStaff = new IdentityRole("OfficeStaff");
                await roleManager.CreateAsync(officeStaff);
            }
            if (!await roleManager.RoleExistsAsync("Tech")) {
                var tech = new IdentityRole("Tech");
                await roleManager.CreateAsync(tech);
            }
            if (!await roleManager.RoleExistsAsync("Manager")) {
                var manager = new IdentityRole("Manager");
                await roleManager.CreateAsync(manager);
            }
        }


        // the belwo two methods are for generating fake test/demo data
        public async Task SeedFakeUsersAsync(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config) {
            var accountsController = new AccountController(userManager, signInManager, config);
            var hasAdmin = Users.Any(x => x.Id == "admin@rtm.com");
            var hasOfficeStaff = Users.Any(x => x.Id == "officestaff@rtm.com");
            var hasTech = Users.Any(x => x.Id == "tech@rtm.com");
            var hasManager = Users.Any(x => x.Id == "manager@rtm.com");

            if (hasAdmin == false){
                var admin = new RegisterUserDTO() {
                    Email = "admin@rtm.com",
                    Password = "Adminpassword-1"
                }; 
                await accountsController.Register(admin);
            }

            if (hasOfficeStaff == false) {
                var officeStaff = new RegisterUserDTO() {
                    Email = "officestaff@rtm.com",
                    Password = "Officestaffpassword-1"
                }; 
                await accountsController.Register(officeStaff);
            }
           
           if (hasTech == false) {
                var tech = new RegisterUserDTO() {
                    Email = "tech@rtm.com",
                    Password = "Techpassword-1"
                }; 
                await accountsController.Register(tech);
            }
            
            if (hasManager == false) {
                var manager = new RegisterUserDTO() {
                    Email = "manager@rtm.com",
                    Password = "Managerpassword-1"
                };
                await accountsController.Register(manager);
            }
        }

        public async Task SeedDevDataAsync() {
            var dataGenerator = new DevDataFaker();
            var jobs = dataGenerator.GenerateJobs();
            var addresses = dataGenerator.GenerateAddresses(jobs);
            var crms = dataGenerator.GenerateCRMs(jobs);
            var vials = dataGenerator.GenerateLSVials(jobs);

            foreach (var job in jobs) {
                Jobs.Add(job);
                await SaveChangesAsync();
            }

            foreach (var address in addresses) {
                Addresses.Add(address);
                await SaveChangesAsync();
            }

            foreach (var crm in crms) {
                ContinuousRadonMonitors.Add(crm);
                await SaveChangesAsync();
            }
            foreach (var vial in vials) {
                LSVials.Add(vial);
                await SaveChangesAsync();
            }
        }
    }


}
