using DocSpot.Core.Contracts;
using DocSpot.Core.Models;
using DocSpot.Core.Models.Req.Appointment;
using DocSpot.Infrastructure.Data.Repository;
using DocSpot.Infrastructure.Data.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocSpot.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/admin/appointments")]
    [Authorize(Roles = Role.Admin)]
    public class AdminAppointmentsController : ControllerBase
    {
        private readonly ILogger<AdminAppointmentsController> logger;
        private readonly IAppointmentsService appointmentService;

        public AdminAppointmentsController(
            ILogger<AdminAppointmentsController> _logger,
            IAppointmentsService service)
        {
            logger = _logger;
            appointmentService = service;
        }

        // GET /api/admin/appointments?from=2026-01-01&to=2026-01-31
        // GET /api/admin/appointments?status=Pending&q=Topchu
        [HttpGet]
        public async Task<ActionResult<AdminAppointmentDto>> GetList([FromQuery] AdminAppointmentsReq req, CancellationToken ct)
        {
            var items = await appointmentService.GetList(req, ct);
            return Ok(items);
        }
    }
}
