namespace DocSpot.Core.Models
{
    public sealed class ExclusionDto
    {
        public required string Id { get; set; }
        public required string Date { get; set; }               // "yyyy-MM-dd"
        public required string ExclusionType { get; set; }      // "Day" | "TimeRange"
        public string? Start { get; set; }                      // "HH:mm" (for TimeRange)
        public string? End { get; set; }                        // "HH:mm" (for TimeRange)
        public string? Reason { get; set; }
    }

    public sealed class CreateExclusionsDto
    {
        public required List<ExclusionDto> Exclusions { get; set; }
    }

    public sealed class QueryExclusionsDto
    {
        public string? From { get; set; }   // "yyyy-MM-dd"
        public string? To { get; set; }     // "yyyy-MM-dd"
    }
}
