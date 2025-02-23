namespace DocSpot.Core.Contracts
{
    using DocSpot.Infrastructure.Data.Models;

    public interface IPatientService
    {
        /// <summary>
        /// Add entity to the database
        /// </summary>
        /// <param name="patient">Entity to add</param>
        /// <returns>The task result contains the number of state entries 
        /// written to the database.</returns>
        public Task<int> CreateAsync(Patient patient);

        public Task<Patient?> GetByIdAsync(string patientId);
    }
}
