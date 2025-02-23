#nullable disable
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

            await userManager.AddToRoleAsync(user, Role.Patient);

            var patient = new Patient()
            {
                UserId = user.Id,
                Name = model.Name,
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

            // Validate password without creating a session
            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(ErrorMessage.InvalidCredentials);

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            // Get user userRoles
            var userRoles = userManager.GetRolesAsync(user).Result;
            var claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add userRoles as claim
            foreach (var role in userRoles)
            {
                claims.Add(new Claim("role", role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
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
