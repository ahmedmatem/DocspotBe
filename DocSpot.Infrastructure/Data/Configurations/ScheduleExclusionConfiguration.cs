using DocSpot.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocSpot.Infrastructure.Data.Configurations
{
    public class ScheduleExclusionConfiguration : IEntityTypeConfiguration<ScheduleExclusion>
    {
        public void Configure(EntityTypeBuilder<ScheduleExclusion> builder)
        {
            builder.HasIndex(x => new { x.Date, x.Start, x.End, x.ExclusionType}).IsUnique();
        }
    }
}
