using AV.Engine.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AV.Engine.Persistence.Database
{
    public class AvDbContext : DbContext
    {
        public AvDbContext(DbContextOptions<AvDbContext> options)
           : base(options) { }

        public DbSet<ScanEvent> ScanEvents { get; set; }
        public DbSet<ThreatInfo> ThreatInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AvDbContext).Assembly);
        }
    }
}
