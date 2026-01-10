using DocSpot.Core.Models.NagePublicHolidays;

namespace DocSpot.Core.Contracts
{
    public interface INagerHolidaysClient
    {
        Task<IReadOnlyList<NagerHolidayDto>> GetPublicHolidaysAsync(
            string countryCode, 
            int year, 
            CancellationToken ct);

        Task<IReadOnlyCollection<DateOnly>> GetLongWeekendDatesAsync(
            string countryCode,
            int year,
            CancellationToken ct);
    }
}
