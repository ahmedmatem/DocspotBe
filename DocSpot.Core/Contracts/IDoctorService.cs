namespace DocSpot.Core.Contracts
{
    using DocSpot.Infrastructure.Data.Models;
    using System.Globalization;

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

        public IQueryable<Doctor> GetDoctorByUserId(string userId);

        public Task<Schedule?> GetScheduleAsync(string doctorId, string date);

        public Task<IEnumerable<Schedule>> GetScheduleRangeAsync(string doctorId, string startDate, string endDate);
    }
}
