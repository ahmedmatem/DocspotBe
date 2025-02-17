namespace DocSpot.Core.Services
{
    using Microsoft.Extensions.Logging;

    using DocSpot.Core.Contracts;
    using DocSpot.Infrastructure.Data.Repository;
    using DocSpot.Infrastructure.Data.Models;

    public class PatientService : IPatientService
    {
        private readonly IRepository repository;

        public PatientService(
            ILogger<PatientService> _logger,
            IRepository _repository)
        {
            repository = _repository;
        }

        public async Task CreateAsync(Patient patient)
        {
            await repository.AddAsync(patient);
            await repository.SaveChangesAsync<Patient>();
        }
    }
}
