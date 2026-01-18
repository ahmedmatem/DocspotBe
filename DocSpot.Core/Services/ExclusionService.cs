using DocSpot.Core.Contracts;
using DocSpot.Core.Extensions;
using DocSpot.Core.Models;
using DocSpot.Infrastructure.Data.Models;
using DocSpot.Infrastructure.Data.Repository;
using DocSpot.Infrastructure.Data.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DocSpot.Core.Services
{
    public class ExclusionService : IExclusionService
    {
        private readonly IRepository repository;

        private readonly ILogger<ExclusionService> logger;

        public ExclusionService(IRepository repo, ILogger<ExclusionService> _logger)
        {
            repository = repo;
            logger = _logger;
        }

        public async Task<int> CreateBatchAsync(CreateExclusionsDto dto, CancellationToken ct = default)
        {
            if (dto.Exclusions is null || dto.Exclusions.Count == 0) return 0;

            var toInsert = new List<ScheduleExclusion>();

            foreach (var e in dto.Exclusions)
            {
                if (!e.Date.TryParseDateOnlyExact(out var date))
                    throw new ArgumentException($"Invalid date '{e.Date}'. Use yyyy-MM-dd.");

                if (!Enum.TryParse<ExclusionType>(e.ExclusionType, ignoreCase: true, out var eType))
                    throw new ArgumentException($"Invalid type '{e.ExclusionType}'. Use Day or TimeRange.");

                TimeSpan? start = null, end = null;

                if (eType == ExclusionType.TimeRange)
                {
                    if (string.IsNullOrWhiteSpace(e.Start) || string.IsNullOrWhiteSpace(e.End))
                        throw new ArgumentException("Start and End are required for TimeRange.");

                    if (!TimeSpan.TryParse(e.Start, out var s) || !TimeSpan.TryParse(e.End, out var en))
                        throw new ArgumentException("Invalid time format. Use HH:mm.");

                    if (s >= en) throw new ArgumentException("Start must be before End.");

                    start = s; end = en;
                }
                else
                {
                    // Day must not carry times
                    if (!string.IsNullOrWhiteSpace(e.Start) || !string.IsNullOrWhiteSpace(e.End))
                    {
                        e.Start = null;
                        e.End = null;
                    }
                }

                toInsert.Add(new ScheduleExclusion
                {
                    Date = date,
                    ExclusionType = eType,
                    Start = start,
                    End = end,
                    Reason = string.IsNullOrWhiteSpace(e.Reason) ? null : e.Reason!.Trim()
                });
            }

            // Use Upsert-like behavior: ignore duplicates by unique index
            // For SQL Server, we can try bulk add and discard conflicts (catch unique violations).
            await repository.AddRangeAsync(toInsert, ct);
            try
            {
                return await repository.SaveChangesAsync<ScheduleExclusion>();
            }
            catch (DbUpdateException ex)
            {
                // de-duplicate by (Date, ExclusionType, Start, End); simplistic retry:
                // Fallback: insert one by one (ignore duplicates)
                int saved = 0;
                foreach (var one in toInsert)
                {
                    try
                    {
                        await repository.AddAsync(one);
                        saved += await repository.SaveChangesAsync<ScheduleExclusion>(ct);
                    }
                    catch (DbUpdateException)
                    {
                        repository.Detach(one); // duplicate -> ignore
                    }
                }

                return saved;
            }
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
        {
            var stub = new ScheduleExclusion { Id = id };
            repository.Attach(stub);
            repository.Delete(stub);
            try
            {
                var affected = await repository.SaveChangesAsync<ScheduleExclusion>(ct);
                return affected > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public Task<List<ScheduleExclusion>> GetAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default)
        {
            var q = repository.AllReadOnly<ScheduleExclusion>();
            if (from.HasValue) q = q.Where(x => x.Date >= from.Value);
            if (to.HasValue) q = q.Where(x => x.Date <= to.Value);
            return q.OrderBy(x => x.Date)
                    .ThenBy(x => x.ExclusionType)
                    .ThenBy(x => x.Start)
                    .ToListAsync(ct);

        }

        // Overlap helper: [slotStart, slotEnd) intersects Day or TimeRange
        public bool IsSlotExcluded(DateOnly date, TimeSpan start, TimeSpan end, IReadOnlyList<ScheduleExclusion> exclusions)
        {
            foreach (var ex in exclusions)
            {
                if (ex.Date != date) continue;

                if (ex.ExclusionType == ExclusionType.Day) return true;

                if (ex.ExclusionType == ExclusionType.TimeRange && ex.Start.HasValue && ex.End.HasValue)
                {
                    // Overlap if start < ex.End && ex.Start < end
                    if (start < ex.End.Value && ex.Start.Value < end)
                        return true;
                }
            }
            return false;
        }
    }
}
