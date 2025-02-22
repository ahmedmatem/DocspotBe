namespace DocSpot.Core.Contracts
{
    using DocSpot.Infrastructure.Data.Models;

    public interface IDoctorService
    {
        /// <summary>
        /// Add entity to the database
        /// </summary>
        /// <param name="schedule">Entity to add</param>
        /// <returns>The task result contains the number of state entries 
        /// written to the database.</returns>
        public Task<int> AddScheduleAsync(Schedule schedule);

        /// <summary>
        /// Add entity to the database
        /// </summary>
        /// <param name="doctor">Entity to add</param>
        /// <returns>The task result contains the number of state entries 
        /// written to the database.</returns>
        public Task<int> CreateAsync(Doctor doctor);
    }
}
