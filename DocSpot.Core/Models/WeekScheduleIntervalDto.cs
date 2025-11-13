using DocSpot.Infrastructure.Data.Types;

namespace DocSpot.Core.Models
{
    public class WeekScheduleIntervalDto
    {
        public DayOfWeekIso Day { get; set; } // enum: Mon, Tue, ...
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
    }
}
