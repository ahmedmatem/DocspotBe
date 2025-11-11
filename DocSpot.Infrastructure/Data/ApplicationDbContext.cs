#nullable disable
namespace DocSpot.Infrastructure.Data
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.Infrastructure.Data.Configurations;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //// data seeding
            builder.ApplyConfiguration(new IdentityRoleConfiguration()); // roles
            builder.ApplyConfiguration(new IdentityUserConfiguration()); // admin/user
            builder.ApplyConfiguration(new IdentityUserRoleConfiguration()); // admin/user -> role

            //// entity configurations
            //builder.ApplyConfiguration(new DoctorEntityConfiguration());
            //builder.ApplyConfiguration(new PatientEntityConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Appointment> Appointments { get; set; }
    }
}
