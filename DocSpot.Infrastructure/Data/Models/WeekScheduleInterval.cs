using DocSpot.Infrastructure.Data.Abstracts;
using DocSpot.Infrastructure.Data.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocSpot.Infrastructure.Data.Models
{
    // day 08:00 12:00
    public class WeekScheduleInterval : BaseModel
    {
        [ForeignKey(nameof(WeekSchedule))]
        public string WeekScheduleId { get; set; } = default!;

        public DayOfWeekIso Day { get; set; } // Sun = 0, Mon, Tue, Wed, Thu, Fri, Sat

        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        // Navigation properties
        public WeekSchedule WeekSchedule { get; set; } = default!;
    }
}
