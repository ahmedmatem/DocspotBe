namespace DocSpot.Core.Contracts
{
    public interface IHolidaysService
    {
        Task<int> SyncYearAsync(string countryCode, int year, CancellationToken ct);
    }
}
