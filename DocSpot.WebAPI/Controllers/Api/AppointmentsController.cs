namespace DocSpot.WebAPI.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using DocSpot.Infrastructure.Data.Types;
    using DocSpot.Core.Contracts;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.Patient)]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> logger;
        private readonly IAppointmentsService appointmentsService;

        public AppointmentsController(
            ILogger<AppointmentsController> _logger,
            IAppointmentsService _appointmentsService)
        {
            logger = _logger;
            appointmentsService = _appointmentsService;
        }

        [HttpGet("occupied-slots/{date}")]
        public async Task<IActionResult> GetOccupiedSlots(string doctorId, string date)
        {
            var occupiedSlots = await appointmentsService.Appointments(doctorId, date);

            return Ok(occupiedSlots);
        }

        [HttpGet("occupied-slots-range/{startDate}/{endDate}")]
        public async Task<IActionResult> GetOccupiedSlotsInRange(
            string doctorId,
            string startDate,
            string endDate)
        {
            var occupiedSlotsInRange = await appointmentsService
                .AppointmentsInRange(doctorId, startDate, endDate);

            return Ok(occupiedSlotsInRange);
        }
    }
}
