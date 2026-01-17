namespace DocSpot.WebAPI.Controllers.Api
{
    using DocSpot.Core.Exceptions;
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Messages;
    using DocSpot.Core.Models;
    using DocSpot.Core.Models.Account;
    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.Infrastructure.Data.Types;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using DocSpot.Core.Extensions;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly PasswordHasher<IdentityUser> passwordHasher;
        private readonly IDoctorService doctorService;
        private readonly IScheduleService scheduleService;

        public AdminController(
            UserManager<IdentityUser> _userManager,
            IDoctorService _doctorService,
            IScheduleService _scheduleService)
        {
            userManager = _userManager;
            doctorService = _doctorService;
            passwordHasher = new PasswordHasher<IdentityUser>();
            scheduleService = _scheduleService;
        }

        // GET api/admin/all-week-schedule
        [HttpGet("all-week-schedule")]
        public async Task<IActionResult> GetAllWeekSchedulesWithIntervals(CancellationToken ct)
        {
            var list = await scheduleService.GetAllWeekSchedulesWithIntervalsAsync(ct);
            return Ok(list);
        }

        // POST /api/admin/week-schedule
        [HttpPost("week-schedule")]
        public async Task<IActionResult> CreateWeekSchedule(WeekScheduleDto dto, CancellationToken ct)
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

        // DELETE /api/admin/week-schedule/{startDate}
        [HttpDelete("week-schedule/{startDate}")]
        public async Task<ActionResult<int>> Delete([FromRoute] string startDate, CancellationToken ct = default)
        {
            if(!startDate.TryParseDateOnlyExact(out var date))
                return BadRequest($"Invalid date: {startDate}. Use yyyy-MM-dd.");

            var ok = await scheduleService.DeleteWeekScheduleAsync(date, ct);
            if (!ok) return NotFound();   // 404 if missing
            return NoContent();           // 204 on success
        }

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
