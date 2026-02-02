using DocSpot.Core.Contracts;
using DocSpot.Core.Models;
using DocSpot.Core.Models.Req.Appointment;
using Microsoft.AspNetCore.Mvc;
using static DocSpot.Core.Models.ActionResult;
using static DocSpot.Core.Models.Req.Appointment.AdminAppointmentsActionReq;

namespace DocSpot.WebAPI.Areas.Admin.Controllers.Api
{
    // api/admin/appointments
    [Route("api/admin/appointments")]
    public class AppointmentsController : BaseAdminController
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IAppointmentsService _service;

        public AppointmentsController(
            ILogger<AppointmentsController> _logger,
            IAppointmentsService service)
        {
            this._logger = _logger;
            this._service = service;
        }

        // GET /api/admin/appointments?from=2026-01-01&to=2026-01-31
        // GET /api/admin/appointments?status=Pending&q=Topchu
        [HttpGet]
        public async Task<ActionResult<AdminAppointmentDto>> GetList([FromQuery] AdminAppointmentsReq req, CancellationToken ct)
        {
            var items = await _service.GetList(req, ct);
            return Ok(items);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id, [FromBody] CancelAppointmentReq req, CancellationToken ct)
        {
            var result = await _service.AdminCancelAsync(id, req, ct);

            return result switch
            {
                AdminActionResult.NotFound => NotFound("Часът не е намерен."),
                AdminActionResult.Conflict => Conflict("Часът не може да бъде отменен."),
                AdminActionResult.Success => Ok("Часът е отменен."),
                _ => BadRequest("Грешка при отмяна на час.")
            };
        }
    }
}
