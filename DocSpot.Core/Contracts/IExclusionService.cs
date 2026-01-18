using DocSpot.Core.Models;
using DocSpot.Infrastructure.Data.Models;

namespace DocSpot.Core.Contracts
{
    public interface IExclusionService
    {
        Task<int> CreateBatchAsync(CreateExclusionsDto dto, CancellationToken ct = default);

        Task<List<ScheduleExclusion>> GetAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default);

        Task<bool> DeleteAsync(string id, CancellationToken ct = default);

        // Helper for filtering slots:
        bool IsSlotExcluded(DateOnly date, TimeSpan start, TimeSpan end, IReadOnlyList<ScheduleExclusion> exclusions);
    }
}
