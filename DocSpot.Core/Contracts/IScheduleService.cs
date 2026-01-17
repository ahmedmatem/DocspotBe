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
        /// Generates all available time slots for a given date based on the active week schedule.
        /// </summary>
        /// <param name="date">
        /// The target date for which to generate time slots, in <c>yyyy-MM-dd</c> format.
        /// </param>
        /// <param name="ct">
        /// A cancellation token that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a read-only list of
        /// <see cref="SlotDto"/> instances representing all generated slots for the specified date.
        /// If there are no working intervals for that day, an empty list is returned.
        /// </returns>
        /// <exception cref="ScheduleValidationException">
        /// Thrown when the <paramref name="date"/> is not in the expected <c>yyyy-MM-dd</c> format,
        /// or when no active week schedule exists for the given date.
        /// </exception>

        public Task<IReadOnlyList<SlotDto>> GetSlotsByDateAsync(string date, CancellationToken ct);

        public Task<bool> DeleteWeekScheduleAsync(DateOnly startDate, CancellationToken ct);
    }
}
