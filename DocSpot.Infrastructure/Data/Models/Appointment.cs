namespace DocSpot.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;

    using DocSpot.Infrastructure.Data.Abstracts;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Appointment : BaseModel
    {

        public required string PatientName { get; set; }

        public required string PatientPhone { get; set; }

        public required string PatientEmail { get; set; }

        public required DateTime AppointmentDate { get; set; }

        public required TimeSpan AppointmentTime { get; set; }

        public string? Message { get; set; }
    }
}
