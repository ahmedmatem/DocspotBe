using DocSpot.Core.Contracts;
using DocSpot.Core.Models;
using DocSpot.Core.Models.Req.Appointment;
using Microsoft.AspNetCore.Mvc;

namespace DocSpot.WebAPI.Areas.Admin.Controllers.Api
{
    // api/admin/appointments
    [Route("api/admin/appointments")]
    public class AppointmentsController : BaseAdminController
    {
        private readonly ILogger<AppointmentsController> logger;
        private readonly IAppointmentsService appointmentService;

        public AppointmentsController(
            ILogger<AppointmentsController> _logger,
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
