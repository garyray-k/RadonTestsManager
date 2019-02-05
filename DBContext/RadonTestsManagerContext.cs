using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RadonTestsManager.CRMs.Models;
using RadonTestsManager.Jobs.Models;
using RadonTestsManager.LSVials.Models;
using RadonTestsManager.Model;
using RadonTestsManager.Utility.Models;

namespace RadonTestsManager.DBContext {
    public class RadonTestsManagerContext : IdentityDbContext<User> {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<ContinuousRadonMonitor> ContinuousRadonMonitors { get; set; }
        public DbSet<LSVial> LSVials { get; set; }
        public DbSet<CRMMaintenanceLogEntry> CRMMaintenanceLogs { get; set; }

        public RadonTestsManagerContext(DbContextOptions<RadonTestsManagerContext> options) : base(options) {
        
        }
        public async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager) {
            if (!await roleManager.RoleExistsAsync("Admin")) {
                var admin = new IdentityRole("Admin");
                await roleManager.CreateAsync(admin);
            }
        }
    }


}
