using DocSpot.Infrastructure.Data.Types;

namespace DocSpot.Core.Models
{
    public class AppointmentPublicDto
    {
        public string Id { get; set; } = null!;
        public string? VisitType { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }

    }
}
