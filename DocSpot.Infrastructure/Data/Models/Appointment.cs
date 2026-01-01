namespace DocSpot.Infrastructure.Data.Models
{
    using DocSpot.Infrastructure.Data.Abstracts;
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

        // Email confirmation status
        public AppointmentStatus AppointmentStatus { get; set; } = AppointmentStatus.Pending;

        // Use to confirm link valid for (e.g.) 2 hours after booking
        // Store hashes only (not raw tokens)
        public string? PublicTokenHash { get; set; }

        public DateTime? TokenExpireAtUtc { get; set; }

        // Cancel token
        public string? CancelTokenHash { get; set; }

        public DateTime? CancelTokenExpireAtUtc { get; set; }

        public DateTime? CancelledAtUtc { get; set; }
    }
}
