using DocSpot.Core.Models;

namespace DocSpot.Core.Contracts
{
    public interface IEmailService
    {
        Task SendAppointmentConfirmationAsync(AppointmentDto appointmentDto, CancellationToken ct);
    }
}
