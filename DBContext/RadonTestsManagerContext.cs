using System;
using Microsoft.EntityFrameworkCore;
using RadonTestsManager.CRMs.Models;
using RadonTestsManager.Jobs.Models;
using RadonTestsManager.LSVials.Models;
using RadonTestsManager.Model;

namespace RadonTestsManager.DBContext {
    public class RadonTestsManagerContext : DbContext {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<ContinuousRadonMonitor> ContinuousRadonMonitors { get; set; }
        public DbSet<LSVial> LSVials { get; set; }

        public RadonTestsManagerContext(DbContextOptions<RadonTestsManagerContext> options) : base(options) {
        
        }
    }
}
