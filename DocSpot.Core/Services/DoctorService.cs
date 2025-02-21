namespace DocSpot.Core.Services
{
    using DocSpot.Core.Contracts;
    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.Infrastructure.Data.Repository;

    public class DoctorService : IDoctorService
    {
        private readonly IRepository repository;

        public DoctorService(
            IRepository _repository)
        {
            repository = _repository;
        }

        /// <summary>
        /// Add entity ti the database
        /// </summary>
        /// <param name="schedule">Entity to add</param>
        /// <returns>The task result contains the number of state entries written to the database.</returns>
        public async Task<int> AddScheduleAsync(Schedule schedule)
        {
            await repository.AddAsync(schedule);
            return await repository.SaveChangesAsync<Schedule>();

        }
    }
}
