namespace DocSpot.Core.Helpers
{
    public static class TimeHelper
    {
        // Use IANA on Linux: "Europe/Sofia"
        // On Windows, this is usually "FLE Standard Time"
        public static TimeZoneInfo GetSofiaTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Sofia"); }
            catch { return TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"); }
        }

        public static DateTime GetAppointmentStartUtc(DateOnly date, TimeOnly time)
        {
            var tz = GetSofiaTimeZone();

            // Unspecified local time (Sofia)
            var local = date.ToDateTime(time); // Kind = Unspecified
            return TimeZoneInfo.ConvertTimeToUtc(local, tz);
        }

        public static DateTime GetCancelDeadlineUtc(DateOnly date, TimeOnly time, int hoursBefore = 3)
            => GetAppointmentStartUtc(date, time).AddHours(-hoursBefore);
    }
}
