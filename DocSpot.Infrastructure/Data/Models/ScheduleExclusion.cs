using DocSpot.Infrastructure.Data.Abstracts;
using DocSpot.Infrastructure.Data.Types;
using System.ComponentModel.DataAnnotations;

namespace DocSpot.Infrastructure.Data.Models
{
    public class ScheduleExclusion : BaseModel
    {
        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public ExclusionType ExclusionType { get; set; }

        // Guard: if ExclusionType == Day then Start/End should be null
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }

        public string? Reason { get; set; }
    }
}
