namespace DocSpot.WebAPI.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using DocSpot.Infrastructure.Data.Types;
    using DocSpot.Core.Models.Account;
    using DocSpot.Core.Messages;
    using DocSpot.Infrastructure.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using DocSpot.Core.Contracts;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly PasswordHasher<IdentityUser> passwordHasher;
        private readonly IDoctorService doctorService;

        public AdminController(
            UserManager<IdentityUser> _userManager,
            IDoctorService _doctorService)
        {
            userManager = _userManager;
            doctorService = _doctorService;
            passwordHasher = new PasswordHasher<IdentityUser>();
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
