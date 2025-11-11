using DocSpot.Infrastructure.Data.Abstracts;
using DocSpot.Infrastructure.Data.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocSpot.Infrastructure.Data.Models
{
    public class WeekScheduleInterval : BaseModel
    {
        [ForeignKey(nameof(WeekSchedule))]
        public string WeekScheduleId { get; set; } = default!;
        public DayOfWeekIso Day { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        // Navigation properties
        public WeekSchedule WeekSchedule { get; set; } = default!;
    }
}
