namespace DocSpot.WebAPI.Controllers.Api
{
    using DocSpot.Core.AppExceptions;
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Messages;
    using DocSpot.Core.Models;
    using DocSpot.Core.Models.Account;
    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.Infrastructure.Data.Types;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("week-schedule")]
        public async Task<IActionResult> CreateWeekSchedule(WeekScheduleDto dto, CancellationToken ct)
        {
            try
            {
                var id = await scheduleService.CreateWeekScheduleAsync(dto, ct);
                // Optionally add a GET and use CreatedAtAction; for now Ok is fine:
                return Ok(new { id });
            }
            catch (ScheduleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
