namespace DocSpot.Core.Models
{
    public class WeekScheduleDto
    {
        public string StartDate { get; set; } = default!; // "yyyy-mm-dd"
        public int SlotLength { get; set; }
        public required Dictionary<string,List<string>> WeekSchedule { get; set; }
    }
}
