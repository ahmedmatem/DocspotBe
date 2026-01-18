namespace DocSpot.WebAPI.Areas.Admin.Controllers.Api
{
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Exceptions;
    using DocSpot.Core.Extensions;
    using DocSpot.Core.Messages;
    using DocSpot.Core.Models;
    using DocSpot.Core.Models.Account;
    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.Infrastructure.Data.Types;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    // api/admin/week-schedules
    [Route("api/admin/week-schedules")]
    public class WeekSchedulesController : BaseAdminController
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly PasswordHasher<IdentityUser> passwordHasher;
        private readonly IDoctorService doctorService;
        private readonly IScheduleService scheduleService;

        public WeekSchedulesController(
            UserManager<IdentityUser> _userManager,
            IDoctorService _doctorService,
            IScheduleService _scheduleService)
        {
            userManager = _userManager;
            doctorService = _doctorService;
            passwordHasher = new PasswordHasher<IdentityUser>();
            scheduleService = _scheduleService;
        }

        // GET api/admin/week-schedules
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await scheduleService.GetAllWeekSchedulesWithIntervalsAsync(ct);
            return Ok(list);
        }

        // POST /api/admin/week-schedules
        [HttpPost]
        public async Task<IActionResult> Create(WeekScheduleDto dto, CancellationToken ct)
        {
            try
            {
                var id = await scheduleService.CreateWeekScheduleAsync(dto, ct);

                return Ok(new { id });
            }
            catch (ScheduleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE /api/admin/week-schedules/{startDate}
        [HttpDelete("{startDate}")]
        public async Task<ActionResult<int>> Delete([FromRoute] string startDate, CancellationToken ct = default)
        {
            if(!startDate.TryParseDateOnlyExact(out var date))
                return BadRequest($"Invalid date: {startDate}. Use yyyy-MM-dd.");

            var ok = await scheduleService.DeleteWeekScheduleAsync(date, ct);
            if (!ok) return NotFound();   // 404 if missing
            return NoContent();           // 204 on success
        }

        // TODO: MOve to DoctorsController (api/admin/doctors)
        [HttpPost("register-doctor")]
        public async Task<IActionResult> RegisterDoctor([FromBody] RegisterModel model)
        {
            var user = new IdentityUser()
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

            var result = await userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(user, Role.Doctor);

            var doctor = new Doctor()
            {
                UserId = user.Id,
                Name = model.Name,
            };

            await doctorService.CreateAsync(doctor);

            return Ok(SuccessMessage.DoctorRegister);
        }

    }
}
