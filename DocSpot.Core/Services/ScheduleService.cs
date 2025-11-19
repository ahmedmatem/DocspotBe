using DocSpot.Core.Contracts;
using DocSpot.Core.Exceptions;
using DocSpot.Core.Models;
using DocSpot.Infrastructure.Data.Models;
using DocSpot.Infrastructure.Data.Repository;
using DocSpot.Infrastructure.Data.Types;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static DocSpot.Core.Constants;

namespace DocSpot.Core.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository repository;

        public ScheduleService(IRepository _repository)
        {
            repository = _repository;
        }

        public async Task<string> CreateWeekScheduleAsync(WeekScheduleDto dto, CancellationToken ct = default)
        {
            if (!DateOnly.TryParse(dto.StartDate, out var startDate))
            {
                throw new ScheduleValidationException("Invalid startDate. Expected yyyy-mm-dd.");
            }

            var exists = await repository
                .AnyAsync<WeekSchedule>(ws => ws.StartDate == startDate, ct);
            if (exists)
            {
                throw new ScheduleValidationException(
                    $"A schedule with startDate {startDate:yyyy-MM-dd} already exists.");
            }

            if (dto.SlotLength <= 0 || dto.SlotLength >= TimeSlotMaxLengthMinutes)
            {
                throw new ScheduleValidationException("Slot length must 0 < be < 60.");
            }

            var intervals = new List<WeekScheduleInterval>();
            foreach (var (dayKey, ranges) in dto.WeekSchedule)
            {
                if (!TryParseDay(dayKey, out var day))
                {
                    throw new ScheduleValidationException($"Invalid day key: '{dayKey}'. Expected mon|tue|wed|thu|fri|sat|sun.");
                }

                foreach (var range in ranges ?? Enumerable.Empty<string>())
                {
                    if (!TryParseRange(range, out var start, out var end))
                    {
                        throw new ScheduleValidationException($"Invalid interval '{range}' for {dayKey}. Use 'HH:mm-HH:mm'.");
                    }

                    if (end <= start)
                    {
                        throw new ScheduleValidationException($"End must be after start for '{range}' on {dayKey}.");
                    }

                    intervals.Add(new WeekScheduleInterval()
                    {
                        Day = day,
                        Start = start,
                        End = end
                    });
                }

                // check overlaps per day
                var dayIntervals = intervals.Where(x => x.Day == day)
                                            .OrderBy(x => x.Start).ToList();
                for (int i = 0; i < dayIntervals.Count - 1; i++)
                    if (dayIntervals[i].End > dayIntervals[i + 1].Start)
                        throw new ScheduleValidationException($"Overlapping intervals on {dayKey}.");
            }

            var entity = new WeekSchedule()
            {
                StartDate = startDate,
                SlotLengthMinutes = dto.SlotLength,
                Intervals = intervals
                    .OrderBy(x => x.Day)
                    .ThenBy(x => x.Start)
                    .ToList()
            };

            await repository.AddAsync(entity);
            await repository.SaveChangesAsync<WeekSchedule>(ct);

            return entity.Id;
        }

        public async Task<IReadOnlyList<WeekScheduleDto>> GetAllWeekSchedulesWithIntervalsAsync(CancellationToken ct = default)
        {
            var entities = await repository.AllReadOnly<WeekSchedule>()
                .Include(ws => ws.Intervals)
                .OrderBy(ws => ws.StartDate) // oldest first
                .ToListAsync(ct);

            var result = new List<WeekScheduleDto>(entities.Count);

            foreach (var weekSchedule in entities)
            {
                // prepare dictionary with all days so frontend can rely on keys being present
                var weekDict = new Dictionary<string, List<string>>()
                {
                    ["mon"] = new(),
                    ["tue"] = new(),
                    ["wed"] = new(),
                    ["thu"] = new(),
                    ["fri"] = new(),
                    ["sat"] = new(),
                    ["sun"] = new(),
                };
                foreach (var interval in weekSchedule.Intervals
                                                        .OrderBy(x => x.Day)
                                                        .ThenBy(x => x.Start))
                {
                    var dayKey = ToDayKey(interval.Day); // 1 -> mon, 2 -> tue, ...
                    weekDict[dayKey].Add(ToRangeString(interval.Start, interval.End));
                }

                result.Add(new WeekScheduleDto
                {
                    StartDate = weekSchedule.StartDate.ToString("yyyy-MM-dd"),
                    SlotLength = weekSchedule.SlotLengthMinutes,
                    WeekSchedule = weekDict
                });
            }

            return result;
        }

        public async Task<IReadOnlyList<SlotDto>> GetSlotsByDate(string date, CancellationToken ct)
        {
            if(!TryParseDayOnly(date, out DateOnly dateOnly))
            {
                throw new ScheduleValidationException($"Date: {date} - Invalid date format. Expected yyyy-MM-dd.");
            }

            var activeWS = await GetActiveWeekScheduleAsync(dateOnly, ct);

            if(activeWS == null)
            {
                throw new ScheduleValidationException($"No active week schedule found for date: {date}");
            }

            // This is the weekday of the *requested date*, not the schedule start
            var requestedIsoDay = ToIsoDay(dateOnly);
            var dayKey = ToDayKey(requestedIsoDay);

            // get only intervals for that weekday
            var intervalsForDay = activeWS.Intervals
                .Where(x => x.Day == requestedIsoDay)
                .OrderBy(x => x.Start)
                .ThenBy(x => x.End)
                .ToList();

            var result = new List<SlotDto>();

            if (!intervalsForDay.Any())
            {
                // no working hours for that day → return empty list
                return result;
            }

            // length of each slot in the interval
            var slotLengthTS = TimeSpan.FromMinutes(activeWS.SlotLengthMinutes);

            foreach (var interval in intervalsForDay)
            {
                // Start from interval.Start, keep adding slotLength
                // until the slot end would go past interval.End.
                for (var current = interval.Start;
                     current + slotLengthTS <= interval.End;
                     current += slotLengthTS)
                {
                    result.Add(new SlotDto
                    {
                        Time = current.ToString(@"hh\:mm"),
                        Available = true,
                        // if needed:
                        // DayKey = dayKey,
                        // Date = dateOnly,
                    });
                }
            }

            return result;
        }

        #region HELPERS

        private Task<WeekSchedule?> GetActiveWeekScheduleAsync(DateOnly date, CancellationToken ct)
        {
            return repository.AllReadOnly<WeekSchedule>()
                .Include(ws => ws.Intervals)
                .Where(ws => ws.StartDate <= date)
                .OrderByDescending(ws => ws.StartDate) // closest, but not after
                .FirstOrDefaultAsync(ct);
        }

        private static bool TryParseRange(string s, out TimeSpan start, out TimeSpan end)
        {
            start = default; end = default;
            if (string.IsNullOrWhiteSpace(s)) return false;

            var parts = s.Split('-', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2) return false;
            return TimeSpan.TryParse(parts[0], out start) &&
                   TimeSpan.TryParse(parts[1], out end);
        }

        private static bool TryParseDay(string key, out DayOfWeekIso day)
        {
            day = key.ToLowerInvariant() switch
            {
                "mon" => DayOfWeekIso.Mon,
                "tue" => DayOfWeekIso.Tue,
                "wed" => DayOfWeekIso.Wed,
                "thu" => DayOfWeekIso.Thu,
                "fri" => DayOfWeekIso.Fri,
                "sat" => DayOfWeekIso.Sat,
                "sun" => DayOfWeekIso.Sun,
                _ => 0
            };
            return day != 0;
        }

        private static string ToDayKey(DayOfWeekIso weekDay) => weekDay switch
        {
            DayOfWeekIso.Mon => "mon",
            DayOfWeekIso.Tue => "tue",
            DayOfWeekIso.Wed => "wed",
            DayOfWeekIso.Thu => "thu",
            DayOfWeekIso.Fri => "fri",
            DayOfWeekIso.Sat => "sat",
            DayOfWeekIso.Sun => "sun",
            _ => throw new ArgumentOutOfRangeException(nameof(weekDay), weekDay, null)
        };

        private static DayOfWeekIso ToIsoDay(DateOnly date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => DayOfWeekIso.Mon,
                DayOfWeek.Tuesday => DayOfWeekIso.Tue,
                DayOfWeek.Wednesday => DayOfWeekIso.Wed,
                DayOfWeek.Thursday => DayOfWeekIso.Thu,
                DayOfWeek.Friday => DayOfWeekIso.Fri,
                DayOfWeek.Saturday => DayOfWeekIso.Sat,
                DayOfWeek.Sunday => DayOfWeekIso.Sun,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string ToDayKey(DateOnly date) => date.DayOfWeek switch
        {
            DayOfWeek.Monday => "mon",
            DayOfWeek.Tuesday => "tue",
            DayOfWeek.Wednesday => "wed",
            DayOfWeek.Thursday => "thu",
            DayOfWeek.Friday => "fri",
            DayOfWeek.Saturday => "sat",
            DayOfWeek.Sunday => "sun",
            _ => throw new ArgumentOutOfRangeException(nameof(date), date, null)
        };

        private static string ToRangeString(TimeSpan start, TimeSpan end)
        {
            // "HH:mm" style formatting for TimeSpan:
            var startStr = start.ToString(@"hh\:mm");
            var endStr = end.ToString(@"hh\:mm");

            return $"{startStr}-{endStr}";
        }

        private static bool TryParseDayOnly(string dateStr, out DateOnly dateOnly)
        {
            return DateOnly.TryParseExact(
                dateStr, 
                "yyyy-MM-dd", 
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, 
                out dateOnly);
        }
        #endregion
    }
}
