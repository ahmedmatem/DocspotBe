namespace DocSpot.WebAPI.Controllers.Api
{
    using AutoMapper;
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Exceptions;
    using DocSpot.Core.Models;
    using DocSpot.Infrastructure.Data.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = Role.Patient)]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> logger;
        private readonly IAppointmentsService appointmentsService;
        private readonly IScheduleService scheduleService;
        private readonly IMapper mapper;

        public AppointmentsController(
            ILogger<AppointmentsController> _logger,
            IAppointmentsService _appointmentsService,
            IScheduleService _scheduleService,
            IMapper _mapper)
        {
            logger = _logger;
            appointmentsService = _appointmentsService;
            scheduleService = _scheduleService;
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
                var slotsForDate = await scheduleService.GetSlotsByDate(date, ct);
                var appointmentsForDate = await appointmentsService.GetAllByDate(date, ct);
                var bookedSlots = appointmentsForDate.Select(a => a.AppointmentTime.ToString("HH:mm"));

                foreach (var slot in slotsForDate)
                {
                    if (bookedSlots.Contains(slot.Time))
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
        public async Task<IActionResult> Book(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            var appointmentDto = mapper.Map<AppointmentDto>(model);
            var id = await appointmentsService.Book(appointmentDto);

            return Ok(id);
        }

        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            await appointmentsService.Cancel(id);

            return Ok();
        }
    }
}
