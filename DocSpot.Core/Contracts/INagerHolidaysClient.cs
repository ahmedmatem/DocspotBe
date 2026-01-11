using DocSpot.Core.Models;

namespace DocSpot.Core.Contracts
{
    public interface INagerHolidaysClient
    {
        Task<IReadOnlyList<NagerHolidays>> GetPublicHolidaysAsync(
            string countryCode, 
            int year, 
            CancellationToken ct);

        Task<IReadOnlyCollection<DateOnly>> GetLongWeekendDatesAsync(
            string countryCode,
            int year,
            CancellationToken ct);
    }
}
