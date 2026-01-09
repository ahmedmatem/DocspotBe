using DocSpot.Core.Contracts;
using DocSpot.Core.Models;
using System.Net.Http.Json;

namespace DocSpot.Core.Services
{
    public class NagerHolidaysClient : INagerHolidaysClient
    {
        private readonly HttpClient httpClient;
        public NagerHolidaysClient(HttpClient _httpClient) => httpClient = _httpClient;

        public async Task<IReadOnlyList<NagerHolidayDto>> GetPublicHolidaysAsync(string countryCode, int year, CancellationToken ct)
        {
            // BaseAddress should be: https://date.nager.at/
            var url = $"api/v3/PublicHolidays/{year}/{countryCode}";
            return await httpClient.GetFromJsonAsync<NagerHolidayDto[]>(url, ct) 
                ?? Array.Empty<NagerHolidayDto>();
        }
    }
}
