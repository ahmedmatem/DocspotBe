namespace DocSpot.Core.Models
{
    public sealed record NagerHolidays
    (
        string date,
        string localName,
        string name,
        bool global,
        string[]? types
    );

    public sealed record NagerLongWeekendDto
    (
        DateOnly StartDate,
        DateOnly EndDate,
        int DayCount,
        bool NeedBridgDay
    );
}
