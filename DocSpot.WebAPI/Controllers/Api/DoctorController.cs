namespace DocSpot.WebAPI.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using AutoMapper;

    using DocSpot.Core.Models;
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Messages;
    using DocSpot.Infrastructure.Data.Types;
    using DocSpot.Infrastructure.Data.Models;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.Doctor)]
    public class DoctorController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IDoctorService doctorService;

        public DoctorController(
            IMapper _mapper,
            IDoctorService _doctorService)
        {
            mapper = _mapper;
            doctorService = _doctorService;
        }

        /// <summary>
        /// Add a schedule
        /// </summary>
        [HttpPost("schedule")]
        public async Task<IActionResult> Schedule([FromBody] ScheduleModel model)
        {
            var schedule = mapper.Map<Schedule>(model);
            await doctorService.AddScheduleAsync(schedule);

            return Ok(SuccessMessage.DoctorAddSchedule);
        }

        [HttpGet("schedule/{date}")]
        public async Task<IActionResult> Schedule(string doctorId, string date)
        {
            var schedule = await doctorService.GetScheduleAsync(doctorId, date);
            if (schedule == null)
                return StatusCode(StatusCodes.Status204NoContent);

            return Ok(schedule);
        }

        [HttpGet("schedule-range/{startDate}-{endDate}")]
        public Task<IActionResult> ScheduleRange(
            string doctorId,
            string startDate,
            string endDate)
        {
            throw new NotImplementedException();
        }
    }
}
