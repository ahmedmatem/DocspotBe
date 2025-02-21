namespace DocSpot.WebAPI.Controllers.Api
{
    using DocSpot.Core.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = "Doctor")]
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        [HttpPost("schedule")]
        public async Task<IActionResult> Schedule([FromBody] ScheduleModel model)
        {

        }
    }
}
