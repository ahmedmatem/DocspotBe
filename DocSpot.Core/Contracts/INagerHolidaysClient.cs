using DocSpot.Core.Models.NagePublicHolidays;
using System.Collections.Generic;

namespace DocSpot.Core.Contracts
{
    public interface INagerHolidaysClient
    {
        Task<IReadOnlyList<NagerHolidayDto>> GetPublicHolidaysAsync(
            string countryCode, 
            int year, 
            CancellationToken ct);
    }
}
