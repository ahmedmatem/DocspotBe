namespace DocSpot.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;

    using DocSpot.Infrastructure.Data.Abstracts;

    public class Schedule : BaseModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        [Comment("The date of the schedule")]
        public required DateTime Date { get; set; }

        [Required]
        public required TimeSpan StartTime { get; set; }

        [Required]
        public required TimeSpan EndTime { get; set; }

        [ForeignKey(nameof(Doctor))]
        public string DoctorId { get; set; } = string.Empty;

        public Doctor Doctor { get; set; } = null!;

        // Navigation properties

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
