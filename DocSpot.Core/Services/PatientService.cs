namespace DocSpot.Core.Services
{
    using Microsoft.Extensions.Logging;

    using DocSpot.Core.Contracts;
    using DocSpot.Infrastructure.Data.Repository;
    using DocSpot.Infrastructure.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class PatientService : IPatientService
    {
        private readonly IRepository repository;

        public PatientService(
            ILogger<PatientService> _logger,
            IRepository _repository)
        {
            repository = _repository;
        }

        /// <summary>
        /// Add entity to the database
        /// </summary>
        /// <param name="patient">Entity to add</param>
        /// <returns>The task result contains the number of state entries 
        /// written to the database.</returns>
        public async Task<int> CreateAsync(Patient patient)
        {
            await repository.AddAsync(patient);
            return await repository.SaveChangesAsync<Patient>();
        }

        public async Task<Patient?> GetByIdAsync(string patientId)
        {
            return await repository.GetByIdAsync<Patient>(patientId);
        }
    }
}
