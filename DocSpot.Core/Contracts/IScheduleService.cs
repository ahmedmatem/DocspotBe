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

        /// <summary>
        /// Gett all weekly schedule including intervals.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<IReadOnlyList<WeekScheduleDto>> GetAllWeekSchedulesWithIntervalsAsync(CancellationToken ct = default);

        /// <summary>
        /// Returns the list of slots for the specified date, indicating availability.
        /// </summary>
        /// <param name="date">
        /// The target date for which to get slots. Expected in ISO 8601 date format: "yyyy-MM-dd".
        /// Implementations should interpret this date according to the schedule's anchoring logic
        /// (e.g., map to the configured week that contains the date).
        /// </param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        public Task<IReadOnlyList<SlotDto>> GetSlotsByDate(string date, CancellationToken ct);
    }
}
