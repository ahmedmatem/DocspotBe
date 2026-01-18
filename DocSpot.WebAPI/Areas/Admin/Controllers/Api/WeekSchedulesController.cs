namespace DocSpot.WebAPI.Areas.Admin.Controllers.Api
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
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
    using Microsoft.EntityFrameworkCore;

    // api/admin/week-schedules
    [Route("api/admin/week-schedules")]
    public class WeekSchedulesController : BaseAdminController
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly PasswordHasher<IdentityUser> passwordHasher;
        private readonly IDoctorService doctorService;
        private readonly IScheduleService scheduleService;
        private readonly IExclusionService exclusionService;
        private readonly IMapper mapper;

        public WeekSchedulesController(
            UserManager<IdentityUser> _userManager,
            IDoctorService _doctorService,
            IScheduleService _scheduleService,
            IExclusionService _exclusionService,
            IMapper _mapper)
        {
            userManager = _userManager;
            doctorService = _doctorService;
            passwordHasher = new PasswordHasher<IdentityUser>();
            scheduleService = _scheduleService;
            exclusionService = _exclusionService;
            mapper = _mapper;
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

        // POST /api/admin/week-schedules/exclusions
        [HttpPost("exclusions")]
        public async Task<IActionResult> CreateExclusions([FromBody] CreateExclusionsDto dto, CancellationToken ct)
        {
            var created = await exclusionService.CreateBatchAsync(dto, ct);
            return created > 0 ? Ok(new { created }) : Ok(new { created = 0 });
        }

        // GET /api/admin/week-schedules/exclusions?from=2026-01-01&to=2026-01-31
        [HttpGet]
        public async Task<IActionResult> GetExclusions([FromQuery] string? from, [FromQuery] string? to, CancellationToken ct)
        {
            DateOnly? f = null, t = null;
            if (DateOnly.TryParse(from, out var f2)) f = f2;
            if (DateOnly.TryParse(to, out var t2)) t = t2;
            var list = await exclusionService.GetAsync(f, t, ct);
            return Ok(list.Select(x => new
            {
                x.Id,
                Date = x.Date.ToString("yyyy-MM-dd"),
                x.ExclusionType,
                Start = x.Start?.ToString(@"hh\:mm"),
                End = x.End?.ToString(@"hh\:mm"),
                x.Reason
            }));
        }

        // DELETE /api/admin/week-schedules/exclusions/{id}
        [HttpDelete("exclusions/{id}")]
        public async Task<IActionResult> DeleteExclusion([FromRoute] string id, CancellationToken ct)
        {
            var ok = await exclusionService.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }

        // TODO: Move to DoctorsController (api/admin/doctors)
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
