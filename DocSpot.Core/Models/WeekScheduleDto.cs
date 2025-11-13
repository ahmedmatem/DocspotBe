namespace DocSpot.Core.Models
{
    public class WeekScheduleDto
    {
        public string StartDate { get; set; } = default!; // "yyyy-mm-dd"
        public int SlotLength { get; set; }

        // { "mon": ["08:00-12:00"], "tue": [], ... }
        public required Dictionary<string,List<string>> WeekSchedule { get; set; }
    }
}
