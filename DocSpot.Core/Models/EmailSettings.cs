namespace DocSpot.Core.Models
{
    public sealed class EmailSettings
    {
        public string FromName { get; set; } = default!;
        public string FromEmail { get; set; } = default!;
        public string SmtpHost { get; set; } = default!;
        public int SmtpPort { get; set; }
        public bool UseStartTls { get; set; } = true;
        public string Username { get; set; } = default!;
        public string AppPassword { get; set; } = default!;
        public string BaseUrl { get; set; } = default!;
    }
}
