using DocSpot.Infrastructure.Data.Models;
using DocSpot.Infrastructure.Data.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocSpot.Infrastructure.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasIndex(x => new { x.AppointmentDate, x.AppointmentTime })
                .IsUnique()
                .HasFilter($"[{nameof(Appointment.AppointmentStatus)}] <> {(int)AppointmentStatus.Cancelled}");
        }
    }
}
