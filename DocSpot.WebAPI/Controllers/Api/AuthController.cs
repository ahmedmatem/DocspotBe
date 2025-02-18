namespace DocSpot.WebAPI.Controllers.Api
{
    using System;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

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
        private readonly IConfiguration configuration;
        private readonly ILogger<AuthController> logger;
        private readonly PasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();

        private readonly IPatientService patientService;

        public AuthController(
            UserManager<IdentityUser> _userManager,
            SignInManager<IdentityUser> _signInManager,
            IConfiguration _configuration,
            ILogger<AuthController> _logger,
            IPatientService _patientService)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            configuration = _configuration;
            logger = _logger;
            patientService = _patientService;
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
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if(user == null)
                return Unauthorized(ErrorMessage.InvalidCredentials);

            var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
                return Unauthorized(ErrorMessage.InvalidCredentials);

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
