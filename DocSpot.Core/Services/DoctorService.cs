namespace DocSpot.Core.Services
{
    using System.Globalization;

    using Microsoft.EntityFrameworkCore;

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
        /// Add entity to the database
        /// </summary>
        /// <param name="schedule">Entity to add</param>
        /// <returns>The task result contains the number of state entries 
        /// written to the database.</returns>
        public async Task<int> AddScheduleAsync(Schedule schedule)
        {
            await repository.AddAsync(schedule);
            return await repository.SaveChangesAsync<Schedule>();

        }

        /// <summary>
        /// Add entity to the database
        /// </summary>
        /// <param name="doctor">Entity to add</param>
        /// <returns>The task result contains the number of state entries 
        /// written to the database.</returns>
        public async Task<int> CreateAsync(Doctor doctor)
        {
            await repository.AddAsync(doctor);
            return await repository.SaveChangesAsync<Doctor>();
        }

        public async Task<Schedule?> GetScheduleAsync(string doctorId, string date)
        {
            var dateTime = DateTime
                .ParseExact(date, Constants.DateTimeFormat, CultureInfo.InvariantCulture);

            return await repository
                .AllReadonly<Schedule>(s => s.Date == dateTime && s.DoctorId == doctorId)
                .Include(s => s.Appointments)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Schedule>> GetScheduleRangeAsync(
            string doctorId, 
            string startDate,
            string endDate)
        {
            var startDateTime = DateTime
                .ParseExact(startDate, Constants.DateTimeFormat, CultureInfo.InvariantCulture);
            var endDateTime = DateTime
                .ParseExact(endDate, Constants.DateTimeFormat, CultureInfo.InvariantCulture);

            return await repository
                .AllReadonly<Schedule>(s => startDateTime <= s.Date && s.Date <= endDateTime)
                .Include(s => s.Appointments)
                .ToListAsync();
        }
    }
}
