using DocSpot.Infrastructure.Data.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace DocSpot.Infrastructure.Data.Models
{
    public class WeekSchedule : BaseModel
    {
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public int SlotLengthMinutes { get; set; }

        // Navigation properties
        public List<WeekScheduleInterval> Intervals { get; set; } = new();
    }
}
