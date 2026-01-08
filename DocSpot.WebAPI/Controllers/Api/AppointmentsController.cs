namespace DocSpot.WebAPI.Controllers.Api
{
    using AutoMapper;
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Exceptions;
    using DocSpot.Core.Helpers;
    using DocSpot.Core.Models;
    using DocSpot.Core.Models.Req.Appointment;
    using DocSpot.Infrastructure.Data.Types;
    using Microsoft.AspNetCore.Mvc;
    using System.Globalization;

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = Role.Patient)]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> logger;
        private readonly IAppointmentsService appointmentsService;
        private readonly IScheduleService scheduleService;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;

        public AppointmentsController(
            ILogger<AppointmentsController> _logger,
            IAppointmentsService _appointmentsService,
            IScheduleService _scheduleService,
            IEmailService _emailService,
            IMapper _mapper)
        {
            logger = _logger;
            appointmentsService = _appointmentsService;
            scheduleService = _scheduleService;
            emailService = _emailService;
            mapper = _mapper;
        }

        //[HttpGet("occupied-slots/{date}")]
        //public async Task<IActionResult> GetOccupiedSlots(string doctorId, string date)
        //{
        //    var occupiedSlots = await appointmentsService.Appointments(doctorId, date);

        //    return Ok(occupiedSlots);
        //}

        //[HttpGet("occupied-slots-range/{startDate}/{endDate}")]
        //public async Task<IActionResult> GetOccupiedSlotsInRange(
        //    string doctorId,
        //    string startDate,
        //    string endDate)
        //{
        //    var occupiedSlotsInRange = await appointmentsService
        //        .AppointmentsInRange(doctorId, startDate, endDate);

        //    return Ok(occupiedSlotsInRange);
        //}

        [HttpGet("time-slots")]
        public async Task<IActionResult> GetSlots(
            [FromQuery] string date, CancellationToken ct)
        {
            try
            {
                var slotsForDate = await scheduleService.GetSlotsByDateAsync(date, ct);
                var appointmentsForDate = await appointmentsService.GetAllByDateAsync(date, ct);
                // All appointments in database are booked except those that are cancelled
                var bookedSlots = appointmentsForDate
                    .Where(a => a.AppointmentStatus != AppointmentStatus.Cancelled)
                    .Select(a => a.AppointmentTime.ToString("HH:mm"));

                foreach (var slot in slotsForDate)
                {

                    if ((IsToday(date) && IsSlotTimePassed(slot.Time)) ||
                        bookedSlots.Contains(slot.Time))
                    {
                        slot.Available = false;
                    }
                }

                return Ok(slotsForDate);
            }
            catch (ScheduleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("book")]
        public async Task<IActionResult> Book(AppointmentViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            var appointmentDto = mapper.Map<AppointmentDto>(model);
            appointmentDto.PublicToken = TokenHelper.GenerateUrlSafeToken();
            appointmentDto.CancelToken = TokenHelper.GenerateUrlSafeToken();
            appointmentDto.AppointmentStatus = AppointmentStatus.Done;

            // Save appointment to DB
            appointmentDto.Id = await appointmentsService.Book(appointmentDto, ct);

            // Send confirmation email
            await emailService.SendAppointmentConfirmationAsync(appointmentDto, ct);

            return Ok(appointmentDto.Id);
        }

        /// <summary>
        /// Retrieves a preview of the appointment cancellation, including any applicable details or consequences, for
        /// the specified appointment request.
        /// </summary>
        /// <param name="req">The appointment cancellation request containing the appointment identifier and any required authentication
        /// information.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the cancellation preview details if the request is valid;
        /// otherwise, a bad request result if the appointment ID or token is invalid.</returns>
        [HttpGet("cancel-preview")]
        public async Task<IActionResult> GetCancelPreviewAsync([FromQuery] AppointmentPublicReq req, CancellationToken ct)
        {
            var appt = await appointmentsService.GetCancelPreviewAsync(req, ct);

            return appt is null
                ? BadRequest("Invalid appointment ID or token.")
                : Ok(appt);
        }

        [HttpGet("public/cancel")]
        public async Task<IActionResult> CancelAsync([FromQuery] AppointmentPublicReq req, CancellationToken ct)
        {
            var result = await appointmentsService.CancelAsync(req, ct);

            return result switch
            {
                //OperationResult.Failed => BadRequest("Invalid appointment ID or token."),
                OperationResult.Success => Ok("Appointment cancelled successfully."),
                _ => BadRequest("Invalid appointment ID or token.")
            };
        }

        private static bool TryParseDateOnly(string dateStr, out DateOnly dateOnly)
        {
            return DateOnly.TryParseExact(
                dateStr,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateOnly);
        }

        private static bool IsToday(string dateStr)
        {
            if (!TryParseDateOnly(dateStr, out DateOnly dateOnly))
            {
                throw new ScheduleValidationException($"Date: {dateStr} - Invalid date format. Expected yyyy-MM-dd.");
            }
            return dateOnly == DateOnly.FromDateTime(DateTime.Today);
        }

        private static bool TryParseTimeOnly(string timeStr, out TimeOnly timeOnly)
        {
            return TimeOnly.TryParseExact(
                timeStr,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out timeOnly);
        }

        private static bool IsSlotTimePassed(string slotTimeStr)
        {
            if (!TryParseTimeOnly(slotTimeStr, out var slot))
                return false;

            var bgTz = GetBgTimeZone();

            // Start from UTC, then convert to BG
            var nowBg = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, bgTz);
            var nowBgTime = TimeOnly.FromDateTime(nowBg.DateTime);

            return slot <= nowBgTime;
        }

        private static TimeZoneInfo GetBgTimeZone()
        {
            // Linux containers usually use IANA ids
            try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Sofia"); }
            catch (TimeZoneNotFoundException) { }

            // Windows uses Windows ids
            return TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        }
    }
}
