namespace DocSpot.Core.Models
{
    public class AppointmentModel
    {
        public required DateTime AppointmentDate { get; set; }

        public required TimeSpan AppointmentTime { get; set; }

        public required string PatientId { get; set; }

        public required string ScheduleId { get; set; }
    }
}
