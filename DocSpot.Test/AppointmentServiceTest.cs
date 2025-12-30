using AutoMapper;
using DocSpot.Core.Models;
using DocSpot.Core.Services;
using DocSpot.Infrastructure.Data;
using DocSpot.Infrastructure.Data.Models;
using DocSpot.Infrastructure.Data.Repository;
using DocSpot.Infrastructure.Data.Types;
using Microsoft.EntityFrameworkCore;

namespace DocSpot.Test
{
    public class AppointmentServiceTest
    {
        private AppointmentsService appointmentsService;
        private ApplicationDbContext inMemoryDbContext;
        private Repository repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "db_" + Guid.NewGuid().ToString())
                .Options;

            inMemoryDbContext = new ApplicationDbContext(options);
            repository = new Repository(inMemoryDbContext);
            appointmentsService = new AppointmentsService(repository, null!);
        }

        [Test]
        public void BookTest()
        {
            // Arrange
            var appointment = new AppointmentDto()
            {
                PatientName = "Ahmed",
                PatientPhone = "1231231",
                PatientEmail = "a@a.b",
                VisitType = VisitType.PAID.ToString(),
                AppointmentDate = new DateOnly(),
                AppointmentTime = new TimeOnly()
            };

            var appointment1 = new AppointmentDto()
            {
                PatientName = "Ahmed",
                PatientPhone = "1231231",
                PatientEmail = "a@a.b",
                VisitType = VisitType.PAID.ToString(),
                AppointmentDate = new DateOnly().AddDays(1),
                AppointmentTime = new TimeOnly()
            };

            // Act
            appointmentsService?.Book(appointment);
            appointmentsService?.Book(appointment1);

            var appList = repository.All<Appointment>().ToList();
            
            Assert.IsNotNull(appList);
            Assert.That(appList.Count, Is.EqualTo(2));
        }

        [TearDown]
        public void TearDown()
        {
            inMemoryDbContext.Dispose();
        }
    }
}