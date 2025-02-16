namespace DocSpot.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;

    using DocSpot.Infrastructure.Data.Abstracts;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Appointment : BaseModel
    {
        [Required]
        [Comment("The date of the appointment")]
        public required DateTime AppointmentDate { get; set; }

        [Required]
        [Comment("The time of the appointment")]
        public required TimeSpan AppointmentTime { get; set; }

        [ForeignKey(nameof(Patient))]
        public required string PatientId { get; set; }
        public required Patient Patient { get; set; }

        [ForeignKey(nameof(Schedule))]
        public required string ScheduleId { get; set; }
        public required Schedule Schedule { get; set; }
    }
}
