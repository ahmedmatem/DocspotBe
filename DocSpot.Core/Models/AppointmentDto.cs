using DocSpot.Infrastructure.Data.Types;

namespace DocSpot.Core.Models
{
    public class AppointmentDto
    {
        public string Id { get; set; } = null!;

        public required string PatientName { get; set; }

        public required string PatientPhone { get; set; }

        public required string PatientEmail { get; set; }

        public required string VisitType { get; set; }

        public required DateOnly AppointmentDate { get; set; }

        public required TimeOnly AppointmentTime { get; set; }

        public string? Message { get; set; }
    }
}
