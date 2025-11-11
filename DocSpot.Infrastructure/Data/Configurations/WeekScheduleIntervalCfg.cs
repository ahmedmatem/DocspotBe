using DocSpot.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocSpot.Infrastructure.Data.Configurations
{
    public class WeekScheduleIntervalCfg : IEntityTypeConfiguration<WeekScheduleInterval>
    {
        public void Configure(EntityTypeBuilder<WeekScheduleInterval> builder)
        {
            builder.Property(x => x.Day).HasConversion<int>();
            builder.Property(x => x.Start).HasColumnType("time").IsRequired();
            builder.Property(x => x.End).HasColumnType("time").IsRequired();

            // Prevent identical duplicates within one schedule
            builder.HasIndex(x => new { x.WeekScheduleId, x.Day, x.Start, x.End }).IsUnique();
        }
    }
}
