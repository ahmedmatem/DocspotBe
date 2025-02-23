namespace DocSpot.WebAPI.Controllers.Api
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using DocSpot.Infrastructure.Data.Types;
    using DocSpot.Core.Contracts;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.Patient)]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> logger;
        private readonly IPatientService patientService;

        public PatientController(
            ILogger<PatientController> _logger,
            IPatientService _patientService)
        {
            logger = _logger;
            patientService = _patientService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInfo(string id)
        {
            var patient = await patientService.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (patient.UserId != userId)
                return Unauthorized();

            return Ok(patient);
        }

        [HttpGet("appointments/{id}")]
        public async Task<IActionResult> Appointments(string id)
        {
            var patient = await patientService.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (patient.UserId != userId)
                return Unauthorized();

            var appointments = await patientService.GetAppointments(id);

            return Ok(appointments);
        }
    }
}
