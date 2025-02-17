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

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new IdentityRoleConfiguration()); // seeds roles

            builder.ApplyConfiguration(new DoctorEntityConfiguration());
            builder.ApplyConfiguration(new PatientEntityConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Schedule> Schedules { get; set; }
    }
}
