namespace DocSpot.Core.Contracts
{
    using DocSpot.Infrastructure.Data.Models;

    public interface IAppointmentsService
    {
        public Task<IEnumerable<Appointment>> Appointments(string doctorId, string date);
    }
}
