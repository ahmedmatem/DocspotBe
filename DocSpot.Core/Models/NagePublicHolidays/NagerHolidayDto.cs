namespace DocSpot.Core.Models.NagePublicHolidays
{
    public sealed record NagerHolidayDto
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
