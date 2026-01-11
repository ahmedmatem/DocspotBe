using DocSpot.Core.Contracts;
using DocSpot.Core.Models;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace DocSpot.Core.Services
{
    public class NagerHolidaysClient : INagerHolidaysClient
    {
        private readonly HttpClient httpClient;
        public NagerHolidaysClient(HttpClient _httpClient) => httpClient = _httpClient;

        public async Task<IReadOnlyList<NagerHolidays>> GetPublicHolidaysAsync(string countryCode, int year, CancellationToken ct)
        {
            // BaseAddress should be: https://date.nager.at/
            var url = $"api/v3/PublicHolidays/{year}/{countryCode}";
            return await httpClient.GetFromJsonAsync<NagerHolidays[]>(url, ct) 
                ?? Array.Empty<NagerHolidays>();
        }
        public async Task<IReadOnlyCollection<DateOnly>> GetLongWeekendDatesAsync(string countryCode, int year, CancellationToken ct)
        {
            // GET https://date.nager.at/api/v3/LongWeekend/{year}/{countryCode}
            var url = $"api/v3/LongWeekend/{year}/{countryCode}";
            var weekends = await httpClient.GetFromJsonAsync<NagerLongWeekendDto[]>(url, ct) 
                ?? [];

            // Expand each weekend: startDate + [0..dayCount-1]
            var dates = new HashSet<DateOnly>();

            foreach (var w in weekends)
            {
                if (w.DayCount <= 0) continue;

                for (var i = 0; i < w.DayCount; i++)
                {
                    dates.Add(w.StartDate.AddDays(i));
                }
            }

            return dates;
        }
    }
}
