using DocSpot.Infrastructure.Data.Models;
using DocSpot.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocSpot.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/holidays")]
    public class HolidaysController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly ILogger<HolidaysController> logger;

        public HolidaysController(
            IRepository _repository,
            ILogger<HolidaysController> _logger)
        {
            repository = _repository;
            logger = _logger;
        }

        // GET /api/holidays/year/2026
        [HttpGet("year/{year:int}")]
        public async Task<ActionResult<IEnumerable<string>>> GetByYear(int year, CancellationToken ct)
        {
            var start = new DateOnly(year, 1, 1);
            var end = new DateOnly(year, 12, 31);

            var dates = await repository.AllReadOnly<Holiday>()
                .Where(h => start <= h.Date && h.Date <= end && h.CountryCode == "BG")
                .Select(h => h.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(ct);

            // Return ISO strings to avoid DateOnly JSON issues
            return Ok(dates.Select(d => d.ToString("yyyy-MM-dd")));
        }

        // GET /api/holidays/upcoming?months=12
        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<string>>> GetUpcoming([FromQuery] int months = 12, CancellationToken ct = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var end = today.AddMonths(months);
            var dates = await repository.AllReadOnly<Holiday>()
                .Where(h => today <= h.Date && h.Date <= end && h.CountryCode == "BG")
                .Select(h => h.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(ct);

            // Return ISO strings to avoid DateOnly JSON issues
            return Ok(dates.Select(d => d.ToString("yyyy-MM-dd")));
        }

        // GET /api/holidays?from=2026-01-01&to=2026-12-31
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetRange(
            [FromQuery] DateOnly from,
            [FromQuery] DateOnly to,
            CancellationToken ct)
        {
            var dates = await repository.AllReadOnly<Holiday>()
                .Where(h => h.CountryCode == "BG" && from <= h.Date && h.Date <= to)
                .Select(h => h.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(ct);

            return Ok(dates.Select(d => d.ToString("yyyy-MM-dd")));
        }
    }
}
