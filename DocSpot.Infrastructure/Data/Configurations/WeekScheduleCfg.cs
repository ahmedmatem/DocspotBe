using DocSpot.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocSpot.Infrastructure.Data.Configurations
{
    public class WeekScheduleCfg : IEntityTypeConfiguration<WeekSchedule>
    {
        public void Configure(EntityTypeBuilder<WeekSchedule> builder)
        {
            builder.HasMany(ws => ws.Intervals)
                .WithOne(wsi => wsi.WeekSchedule)
                .HasForeignKey(wsi => wsi.WeekScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Business rule example: only one schedule per StartDate
            builder.HasIndex(ws => ws.StartDate).IsUnique();
        }
    }
}
