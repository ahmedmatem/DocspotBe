namespace DocSpot.Core.Contracts
{
    using DocSpot.Core.Models;
    using DocSpot.Infrastructure.Data.Models;

    public interface IAppointmentsService
    {
        //public Task<IEnumerable<Appointment>> Appointments(string doctorId, string date);

        //public Task<IEnumerable<Appointment>> AppointmentsInRange(string doctorId, string startDate, string endDate);

        public Task Book(Appointment appointment);

        public Task Cancel(string appointmentId);

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
