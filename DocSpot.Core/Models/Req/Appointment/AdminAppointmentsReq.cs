namespace DocSpot.Core.Models.Req.Appointment
{
    public sealed class AdminAppointmentsReq
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Status { get; set; }
        public string? Query { get; set; }
    }
}
