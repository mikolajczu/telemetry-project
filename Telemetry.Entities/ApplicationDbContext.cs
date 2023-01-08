using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using Telemetry.Entities.Configurations;
using Telemetry.Entities.Models;

namespace Telemetry.Entities
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Page> Pages { get; set; }
        public DbSet<UserPage> UserPages { get; set; }
        public DbSet<TelemetrySession> TelemetrySessions { get; set; }

        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("User ID=testuser;Password=testuser;Host=localhost;Port=5432;Database=telemetry;Pooling=true;");
            optionsBuilder.UseLazyLoadingProxies(true);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new PageConfiguration());
            builder.ApplyConfiguration(new TelemetrySessionConfiguration());
            builder.ApplyConfiguration(new UserPageConfiguration());
            base.OnModelCreating(builder);
        }
    }
}