using DocSpot.Core.Exceptions;
using DocSpot.Core.Models;
using DocSpot.Core.Services;
using DocSpot.Infrastructure.Data;
using DocSpot.Infrastructure.Data.Models;
using DocSpot.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace DocSpot.Tests.Services
{
    public class ScheduleServiceTest
    {
        private ScheduleService _service;
        private Repository _repo;
        private ApplicationDbContext _dbContext;
        
        [SetUp]
        public void Setup()
        {
            var _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "db_" + Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(_dbOptions);
            _repo = new Repository(_dbContext);
            _service = new ScheduleService(_repo);
        }

        [Test]
        public async Task ReadScheduleWorksCorrectly()
        {
            var schedule = new WeekSchedule 
            {
                StartDate = new DateOnly(),
                SlotLengthMinutes = 20,
                Intervals = new List<WeekScheduleInterval>()
                {
                    new WeekScheduleInterval{ 
                        Day = Infrastructure.Data.Types.DayOfWeekIso.Mon,
                        Start = new TimeSpan(8, 0, 0),
                        End = new TimeSpan(10, 0, 0)
                    }
                }
            };

            await _repo.AddAsync<WeekSchedule>(schedule);
            await _repo.SaveChangesAsync<WeekSchedule>();


            var list = await _service.GetAllWeekSchedulesWithIntervalsAsync();

            Assert.That(list.Count, Is.EqualTo(1));
        }

        [Test]
        public void CreateSchedule_WithIncorrectFormatOfStartDate_ThrowException()
        {
            var fakeStartDateFormat = "2025-13-01";
            var schedule = new WeekScheduleDto
            {
                StartDate = fakeStartDateFormat, // yyyy-mm-dd
                SlotLength = 20,
                WeekSchedule = new Dictionary<string, List<string>>()
                {
                    ["mon"] = new() { "08:00-10:00", "11:00-12:00" }
                }
            };

            var exc = Assert.ThrowsAsync<ScheduleValidationException>(() =>
                _service.CreateWeekScheduleAsync(schedule)
            );
            

            //var list = await _repo.AllReadOnly<WeekSchedule>()
            //    .ToListAsync();

            //Assert.That(list.Count, Is.EqualTo(1));
        }

        [Test]
        public void CreateSchedule_WithNegativeTimeSlot_ThrowException()
        {
            var negativeTimeSlot = -30;
            var schedule = new WeekScheduleDto
            {
                StartDate = "2025-01-20", // yyyy-mm-dd
                SlotLength = negativeTimeSlot,
                WeekSchedule = new Dictionary<string, List<string>>()
                {
                    ["mon"] = new() { "08:00-10:00", "11:00-12:00" }
                }
            };

            var exc = Assert.ThrowsAsync<ScheduleValidationException>(() =>
                _service.CreateWeekScheduleAsync(schedule)
            );
        }

        [TestCase(0)]
        [TestCase(60)]
        [TestCase(-5)]
        [TestCase(65)]
        public void CreateSchedule_WithTimeSlotOutOfRange_ThrowException(int outOfRangeTimeSlot)
        {
            var schedule = new WeekScheduleDto
            {
                StartDate = "2025-01-20", // yyyy-mm-dd
                SlotLength = outOfRangeTimeSlot,
                WeekSchedule = new Dictionary<string, List<string>>()
                {
                    ["mon"] = new() { "08:00-10:00", "11:00-12:00" }
                }
            };

            var exc = Assert.ThrowsAsync<ScheduleValidationException>(() =>
                _service.CreateWeekScheduleAsync(schedule)
            );
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}