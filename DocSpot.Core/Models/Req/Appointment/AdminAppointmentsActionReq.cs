namespace DocSpot.Core.Models.Req.Appointment
{
    public static class AdminAppointmentsActionReq
    {
        public sealed record CancelAppointmentReq(string? Reason, bool NotifyPatient = true);

        public sealed record RescheduleAppointmentReq(
            string NewDate,   // "yyyy-MM-dd"
            string NewTime,   // "HH:mm"
            string? Reason,
            bool NotifyPatient = true
        );
    }
}
