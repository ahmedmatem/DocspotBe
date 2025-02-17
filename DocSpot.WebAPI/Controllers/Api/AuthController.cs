namespace DocSpot.WebAPI.Controllers.Api
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using DocSpot.Core.Messages;
    using DocSpot.Core.Models.Account;
    using DocSpot.Infrastructure.Data;
    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.Infrastructure.Data.Types;
    using DocSpot.Core.Contracts;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IPatientService patientService;
        private readonly ILogger<AuthController> logger;

        public AuthController(
            UserManager<IdentityUser> _userManager,
            SignInManager<IdentityUser> _signInManager,
            IPatientService _patientService,
            ILogger<AuthController> _logger)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            patientService = _patientService;
            logger = _logger;
        }

        /// <summary>
        /// Used to register patients.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new IdentityUser() 
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            var result = await userManager.CreateAsync(user);

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(user, Role.Patient.ToString());

            var patient = new Patient()
            {
                Name = model.Name,
                UserId = user.Id
            };

            await patientService.CreateAsync(patient);

            return Ok(SuccessMessage.PatientRegister);
        }
    }
}
