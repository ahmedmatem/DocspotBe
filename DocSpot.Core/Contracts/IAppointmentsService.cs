namespace DocSpot.Core.Contracts
{
    using DocSpot.Core.Models;
    using DocSpot.Core.Models.Req.Appointment;
    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.Infrastructure.Data.Types;

    public interface IAppointmentsService
    {
        //public Task<IEnumerable<Appointment>> Appointments(string doctorId, string date);

        //public Task<IEnumerable<Appointment>> AppointmentsInRange(string doctorId, string startDate, string endDate);

        public Task<string> Book(AppointmentDto appointmentDto, CancellationToken ct);

        /// <summary>
        /// Retrieves a preview of the appointment cancellation, including any effects or details that would result from
        /// canceling the specified appointment.
        /// </summary>
        /// <param name="req">The request containing the details of the appointment to preview for cancellation. Cannot be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an AppointmentPublicDto with the
        /// cancellation preview details, or null if the appointment cannot be found or previewed.</returns>
        public Task<AppointmentPublicDto?> GetCancelPreviewAsync(AppointmentPublicReq req, CancellationToken ct);

        /// <summary>
        /// Cancels an existing appointment based on the specified request parameters.
        /// </summary>
        /// <param name="req">An object containing the details of the appointment to cancel. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous cancel operation.</returns>
        public Task<OperationResult> Cancel(AppointmentPublicReq req, CancellationToken ct);

        /// <summary>
        /// Gets all appointments scheduled for the specified date.
        /// </summary>
        /// <param name="dateStr">
        /// The date for which to retrieve appointments, in <c>yyyy-MM-dd</c> format.
        /// </param>
        /// <param name="ct">
        /// A cancellation token that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a read-only list of
        /// <see cref="AppointmentDto"/> instances scheduled on the specified date.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown if <paramref name="date"/> is not in the expected <c>yyyy-MM-dd</c> format.
        /// </exception>
        public Task<IReadOnlyList<AppointmentDto>> GetAllByDate(string dateStr, CancellationToken ct);
    }
}
