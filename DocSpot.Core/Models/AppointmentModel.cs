namespace DocSpot.Core.Models
{
    public class AppointmentModel
    {
        public required string PatientName { get; set; }

        public required string PatientPhone { get; set; }

        public required string PatientEmail { get; set; }

        public required DateTime AppointmentDate { get; set; }

        public required TimeSpan AppointmentTime { get; set; }

        public string? Message { get; set; }
    }
}
