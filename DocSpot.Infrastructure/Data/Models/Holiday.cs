namespace DocSpot.Infrastructure.Data.Models
{
    public sealed class Holiday
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string CountryCode { get; set; } = "BG";

        public DateOnly Date { get; set; } // yyyy-MM-dd

        public string LocalName { get; set; } = default!;
        public string Name { get; set; } = default!;

        public bool IsGlobal { get; set; }
        public string? Type { get; set; }
    }
}
