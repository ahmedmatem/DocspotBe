using DocSpot.Infrastructure.Data.Types;

namespace DocSpot.Core.Models
{
    public sealed record AdminAppointmentDto
    (
        string Id,
        string PatientName,
        string PatientPhone,
        string PatientEmail,
        string VisitType,
        string AppointmentDate,   // yyyy-MM-dd
        string AppointmentTime,   // HH:mm
        string? Message,
        AppointmentStatus AppointmentStatus,
        DateTime? CancelledAtUtc
    );
}
