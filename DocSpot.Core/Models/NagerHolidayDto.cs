namespace DocSpot.Core.Models
{
    public sealed record NagerHolidayDto
    (
        string date,
        string localName,
        string name,
        bool global,
        string[]? types
    );
}
