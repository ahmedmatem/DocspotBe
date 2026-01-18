namespace DocSpot.WebAPI.Controllers.Api
{
    using AutoMapper;
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Exceptions;
    using DocSpot.Core.Extensions;
    using DocSpot.Core.Helpers;
    using DocSpot.Core.Models;
    using DocSpot.Core.Models.Req.Appointment;
    using DocSpot.Infrastructure.Data.Types;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Globalization;

    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> logger;
        private readonly IAppointmentsService appointmentsService;
        private readonly IScheduleService scheduleService;
        private readonly IExclusionService exclusionService;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;

        public AppointmentsController(
            ILogger<AppointmentsController> _logger,
            IAppointmentsService _appointmentsService,
            IScheduleService _scheduleService,
            IExclusionService _exclusionService,
            IEmailService _emailService,
            IMapper _mapper)
        {
            logger = _logger;
            appointmentsService = _appointmentsService;
            scheduleService = _scheduleService;
            exclusionService = _exclusionService;
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

        // GET /api/appointments/2026-01-18/slots
        [HttpGet("{date}/slots")]
        public async Task<IActionResult> GetSlots([FromRoute] string date, CancellationToken ct)
        {
            try
            {
                var slotsForDate = await scheduleService.GetSlotsByDateAsync(date, ct);
                var appointmentsForDate = await appointmentsService.GetAllByDateAsync(date, ct);
                // All appointments in database are booked except those that are cancelled
                var bookedSlots = appointmentsForDate
                    .Where(a => a.AppointmentStatus != AppointmentStatus.Cancelled)
                    .Select(a => a.AppointmentTime.ToString("HH:mm"))
                    .ToHashSet(StringComparer.Ordinal); // faster contains

                // Parse yyyy-MM-dd safely
                if (!date.TryParseDateOnlyExact(out var dateOnly))
                {
                    return BadRequest(new { error = $"Invalid date: ${date}. Use yyyy-MM-dd." });
                }

                var exclusions = await exclusionService.GetAsync(dateOnly, dateOnly, ct);

                // Whole-day exclusion -> everything unavailable (return Ok([]))
                if (exclusions.Any(e => e.ExclusionType == ExclusionType.Day))
                {
                    return Ok(Array.Empty<SlotDto>());
                    // or set all slots as unavailable
                    //foreach (var s in slotsForDate) s.Available = false;
                    //return Ok(slotsForDate);
                }

                // Set passed and booked slots as unavailable
                foreach (var slot in slotsForDate)
                {
                    if ((date.IsToday() && slot.Time.IsTimePassed()) ||
                        bookedSlots.Contains(slot.Time))
                    {
                        slot.Available = false;
                    }
                }

                // Filter excluded slots
                var filtered = slotsForDate.Where(s =>
                {
                    var start = TimeSpan.ParseExact(s.Time, @"hh\:mm", CultureInfo.InvariantCulture);
                    var end = start.Add(TimeSpan.FromMinutes(s.Length));
                    return !exclusionService.IsSlotExcluded(dateOnly, start, end, exclusions);
                });

                return Ok(filtered);
            }
            catch (ScheduleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST /api/appointments
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentViewModel model, CancellationToken ct)
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

        // GET /api/appointments/cancellation/preview?id=...&token=...
        [HttpGet("cancellation/preview")]
        public async Task<IActionResult> GetCancelPreviewAsync([FromQuery] AppointmentPublicReq req, CancellationToken ct)
        {
            var appt = await appointmentsService.GetCancelPreviewAsync(req, ct);

            return appt is null
                ? BadRequest("Invalid appointment ID or token.")
                : Ok(appt);
        }

        // POST /api/appointments/cancellation?id=...&token=...
        [HttpPost("cancellation")]
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
    }
}
