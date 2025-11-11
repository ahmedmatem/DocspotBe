using DocSpot.Core.Models;

namespace DocSpot.Core.Contracts
{
    public interface IScheduleService
    {
        /// <summary>
        /// Creates a weekly schedule and returns its Id.
        /// Throws ScheduleValidationException for bad input.
        /// </summary>
        public Task<string> CreateWeekScheduleAsync(WeekScheduleDto dto, CancellationToken ct = default);
    }
}
