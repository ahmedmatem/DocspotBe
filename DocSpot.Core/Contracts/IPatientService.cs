namespace DocSpot.Core.Contracts
{
    using DocSpot.Infrastructure.Data.Models;

    public interface IPatientService
    {
        public Task CreateAsync(Patient patient);
    }
}
