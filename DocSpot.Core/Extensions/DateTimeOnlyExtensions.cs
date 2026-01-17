using DocSpot.Core.Exceptions;
using System;
using System.Globalization;

namespace DocSpot.Core.Extensions
{
    public static class DateTimeOnlyExtensions
    {
        // ---------- Parsing helpers (string -> DateOnly / TimeOnly) ----------

        public static bool TryParseDateOnlyExact(this string? value, out DateOnly date,
            string format = "yyyy-MM-dd",
            IFormatProvider? provider = null,
            DateTimeStyles styles = DateTimeStyles.None)
        {
            provider ??= CultureInfo.InvariantCulture;
            return DateOnly.TryParseExact(value, format, provider, styles, out date);
        }

        public static bool TryParseTimeOnlyExact(this string? value, out TimeOnly time,
            string format = "HH:mm",
            IFormatProvider? provider = null,
            DateTimeStyles styles = DateTimeStyles.None)
        {
            provider ??= CultureInfo.InvariantCulture;
            return TimeOnly.TryParseExact(value, format, provider, styles, out time);
        }

        public static DateOnly? ToDateOnlyOrNull(this string? value, string format = "yyyy-MM-dd",
            IFormatProvider? provider = null, DateTimeStyles styles = DateTimeStyles.None)
            => value.TryParseDateOnlyExact(out var d, format, provider, styles) ? d : null;

        public static TimeOnly? ToTimeOnlyOrNull(this string? value, string format = "HH:mm",
            IFormatProvider? provider = null, DateTimeStyles styles = DateTimeStyles.None)
            => value.TryParseTimeOnlyExact(out var t, format, provider, styles) ? t : null;

        // ---------- Time zone helpers (BG safe) ----------

        /// <summary>
        /// Bulgaria timezone (works on Linux containers + Windows).
        /// Linux: "Europe/Sofia", Windows: "FLE Standard Time"
        /// </summary>
        public static TimeZoneInfo GetBgTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Sofia"); }
            catch (TimeZoneNotFoundException) { }
            return TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        }

        public static DateTimeOffset NowIn(this TimeZoneInfo tz)
            => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tz);

        public static TimeOnly TimeNowIn(this TimeZoneInfo tz)
            => TimeOnly.FromDateTime(tz.NowIn().DateTime);

        public static DateOnly TodayIn(this TimeZoneInfo tz)
            => DateOnly.FromDateTime(tz.NowIn().DateTime);

        // ---------- Transformations (DateOnly + TimeOnly -> DateTime / DTO) ----------

        /// <summary>
        /// Combines DateOnly + TimeOnly as a local "wall-clock" DateTime (Unspecified kind).
        /// </summary>
        public static DateTime ToLocalDateTime(this DateOnly date, TimeOnly time)
            => date.ToDateTime(time, DateTimeKind.Unspecified);

        /// <summary>
        /// Combines DateOnly + TimeOnly into a DateTimeOffset in a given timezone.
        /// Correctly applies the zone offset for that local time (incl. DST rules).
        /// </summary>
        public static DateTimeOffset ToDateTimeOffset(this DateOnly date, TimeOnly time, TimeZoneInfo tz)
        {
            var local = date.ToLocalDateTime(time);
            var offset = tz.GetUtcOffset(local);
            return new DateTimeOffset(local, offset);
        }

        // ---------- Business checks (slot passed) ----------
        
        /// <summary>
        /// Checks if slot time (HH:mm) has passed "today" in the given timezone.
        /// Note: this ignores date; use IsSlotPassed(date, timeStr, tz) if you have the date.
        /// </summary>
        //public static bool IsSlotTimePassedToday(this string? slotTimeStr, TimeZoneInfo tz, string format = "HH:mm")
        //{
        //    if (!slotTimeStr.TryParseTimeOnlyExact(out var slot, format))
        //        return false;

        //    var now = tz.TimeNowIn();
        //    return slot <= now;
        //}

        /// <summary>
        /// Checks if a specific date+time slot has passed, in the given timezone.
        /// Recommended (date-aware).
        /// </summary>
        //public static bool IsSlotPassed(this (DateOnly Date, string SlotTimeStr) slot, TimeZoneInfo tz, string format = "HH:mm")
        //{
        //    if (!slot.SlotTimeStr.TryParseTimeOnlyExact(out var time, format))
        //        return false;

        //    var slotDto = slot.Date.ToDateTimeOffset(time, tz);
        //    var now = tz.NowIn();
        //    return slotDto <= now;
        //}

        /// <summary>
        /// Convenience overload: date + TimeOnly.
        /// </summary>
        //public static bool IsSlotPassed(this (DateOnly Date, TimeOnly Time) slot, TimeZoneInfo tz)
        //{
        //    var slotDto = slot.Date.ToDateTimeOffset(slot.Time, tz);
        //    var now = tz.NowIn();
        //    return slotDto <= now;
        //}

        // ---------- "Is today?" checks (timezone-safe) ----------

        //public static bool IsTodayIn(this DateOnly date, TimeZoneInfo tz)
        //    => date == tz.TodayIn();

        public static bool IsToday(this string dateStr, string format = "yyyy-MM-dd")
        {
            if (!DateOnly.TryParseExact(
                dateStr,
                format,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dateOnly))
            {
                return false;
            }
            return dateOnly == DateOnly.FromDateTime(DateTime.Today);
        }

        public static bool IsTimePassed(this string timeStr, string format = "HH:mm")
        {
            if (!TimeOnly.TryParseExact(
                timeStr,
                format,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var timeOnly))
                return false;

            var bgTz = GetBgTimeZone();

            // Start from UTC, then convert to BG
            var nowBg = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, bgTz);
            var nowBgTime = TimeOnly.FromDateTime(nowBg.DateTime);

            return timeOnly <= nowBgTime;
        }
    }
}
