using DocSpot.Core.Contracts;
using DocSpot.Infrastructure.Data.Models;
using DocSpot.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace DocSpot.Core.Services
{
    public class HolidaysService : IHolidaysService
    {
        private readonly INagerHolidaysClient http;
        private readonly IRepository repository;
        private readonly ILogger<HolidaysService> logger;

        public HolidaysService(
            INagerHolidaysClient _http,
            IRepository repo,
            ILogger<HolidaysService> _logger)
        {
            http = _http;
            repository = repo;
            logger = _logger;
        }

        public async Task<int> SyncYearAsync(string countryCode, int year, CancellationToken ct)
        {
            var holidays = await http.GetPublicHolidaysAsync(countryCode, year, ct);
            var longWeekends = await http.GetLongWeekendDatesAsync(countryCode, year, ct);

            var mapped = new List<Holiday>(holidays.Count);
            // Map holidays to entities
            foreach (var holiday in holidays)
            {
                if (!DateOnly.TryParseExact(
                    holiday.date,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dateOnly)) { continue; }                    

                mapped.Add(new Holiday
                {
                    CountryCode = countryCode,
                    Date = dateOnly,
                    LocalName = holiday.localName,
                    Name = holiday.name,
                    IsGlobal = holiday.global,
                    Type = holiday.types is { Length: > 0 } ? string.Join(",", holiday.types) : null
                });
            }

            // Map longWeekends to entities
            foreach (var date in longWeekends)
            {
                if (mapped.Any(h => h.Date == date)) { continue; }

                mapped.Add(new Holiday
                {
                    CountryCode = countryCode,
                    Date = date,
                    LocalName = "",
                    Name = "",
                    IsGlobal = false,
                });
            }


            // Upsert (simple + reliable)
            // Load existing for that year & country in one query
            var start = new DateOnly(year, 1, 1);
            var end = new DateOnly(year, 12, 31);

            var existing = await repository.AllReadOnly<Holiday>()
                .Where(x => x.CountryCode == countryCode && x.Date >= start && x.Date <= end)
                .ToListAsync(ct);

            int changes = 0;

            foreach (var incoming in mapped)
            {
                var found = existing.FirstOrDefault(x => x.Date == incoming.Date && x.Name == incoming.Name);

                if (found is null)
                {
                    //_db.Holidays.Add(incoming);
                    await repository.AddAsync<Holiday>(incoming);
                    changes++;
                    continue;
                }

                // Update fields if changed
                if (found.LocalName != incoming.LocalName) { found.LocalName = incoming.LocalName; changes++; }
                if (found.IsGlobal != incoming.IsGlobal) { found.IsGlobal = incoming.IsGlobal; changes++; }
                if (found.Type != incoming.Type) { found.Type = incoming.Type; changes++; }
            }

            if (changes > 0) await repository.SaveChangesAsync<Holiday>(ct);

            logger.LogInformation($"Holiday sync done: {countryCode} {year}, items={mapped.Count}, changes={changes}");

            return changes;
        }
    }
}
