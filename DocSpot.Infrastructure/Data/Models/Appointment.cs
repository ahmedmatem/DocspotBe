namespace DocSpot.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;

    using DocSpot.Infrastructure.Data.Abstracts;
    using System.ComponentModel.DataAnnotations.Schema;
    using DocSpot.Infrastructure.Data.Types;

    public class Appointment : BaseModel
    {

        public required string PatientName { get; set; }

        public required string PatientPhone { get; set; }

        public required string PatientEmail { get; set; }

        public required VisitType VisitType { get; set; }

        public required DateOnly AppointmentDate { get; set; }

        public required TimeOnly AppointmentTime { get; set; }

        public string? Message { get; set; }
    }
}
