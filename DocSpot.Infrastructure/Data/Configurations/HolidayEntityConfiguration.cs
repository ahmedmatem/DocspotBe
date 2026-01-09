using DocSpot.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocSpot.Infrastructure.Data.Configurations
{
    public class HolidayEntityConfiguration : IEntityTypeConfiguration<Holiday>
    {
        public void Configure(EntityTypeBuilder<Holiday> builder)
        {
            builder.HasIndex(h => new { h.CountryCode, h.Date, h.Name }).IsUnique(); // one holiday per day
        }
    }
}
