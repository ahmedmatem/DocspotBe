namespace DocSpot.WebAPI.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using DocSpot.Infrastructure.Data.Types;
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Models;
    using AutoMapper;
    using DocSpot.Infrastructure.Data.Models;
    using Microsoft.AspNetCore.Http.HttpResults;

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = Role.Patient)]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> logger;
        private readonly IAppointmentsService appointmentsService;
        private readonly IMapper mapper;

        public AppointmentsController(
            ILogger<AppointmentsController> _logger,
            IAppointmentsService _appointmentsService,
            IMapper _mapper)
        {
            logger = _logger;
            appointmentsService = _appointmentsService;
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

        [HttpPost("book")]
        public async Task<IActionResult> Book(AppointmentModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            var appointment = mapper.Map<Appointment>(model);
            await appointmentsService.Book(appointment);

            return Ok(appointment.Id);
        }

        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            await appointmentsService.Cancel(id);

            return Ok();
        }
    }
}
