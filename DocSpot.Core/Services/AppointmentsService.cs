namespace DocSpot.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Models;
    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.Infrastructure.Data.Repository;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public class AppointmentsService : IAppointmentsService
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;

        public AppointmentsService(IRepository _repository, IMapper _mapper)
        {
            repository = _repository;
            mapper = _mapper;
        }

        /// <summary>
        /// Get only all appointments for given date and for schedule that belongs to the doctor with doctorId.
        /// </summary>
        /// <param name="doctorId">The identifier of the doctor which the schedule belongs to.</param>
        /// <param name="date">The date which the appointments belong to.</param>
        /// <returns>Appointments for given doctor and date.</returns>
        //public async Task<IEnumerable<Appointment>> Appointments(string doctorId, string date)
        //{
        //    var dateTime = DateTime.ParseExact(
        //        date, Constants.DateTimeFormat, CultureInfo.InvariantCulture);

        //    return await repository.AllReadOnly<Appointment>()
        //        .Join(repository.AllReadOnly<Schedule>(),
        //            app => app.ScheduleId,
        //            sch => sch.Id,
        //            (appointment, schedule) => new { appointment, schedule })
        //        .Where(
        //            joined => joined.appointment.AppointmentDate == dateTime
        //            && joined.schedule.DoctorId == doctorId)
        //        .Select(joined => joined.appointment)
        //        .OrderBy(app => app.AppointmentTime)
        //        .ToListAsync();
        //}

        /// <summary>
        /// Get only all appointments for given range [startDate, endDate] and for schedule that belongs to the doctor with doctorId.
        /// </summary>
        /// <param name="doctorId">The identifier of the doctor which the schedule belongs to.</param>
        /// <param name="startDate">The start date in the range.</param>
        /// <param name="endDate">The end date in the range.</param>
        /// <returns>Appointments for given doctor within date-range.</returns>
        //public async Task<IEnumerable<Appointment>> AppointmentsInRange(
        //    string doctorId,
        //    string startDate,
        //    string endDate)
        //{
        //    var startDateTime = DateTime.ParseExact(
        //        startDate, Constants.DateTimeFormat, CultureInfo.InvariantCulture);
        //    var endDateTime = DateTime.ParseExact(
        //        endDate, Constants.DateTimeFormat, CultureInfo.InvariantCulture);

        //    return await repository.AllReadOnly<Appointment>()
        //        .Join(repository.AllReadOnly<Schedule>(),
        //            app => app.ScheduleId,
        //            sch => sch.Id,
        //            (appointment, schedule) => new { appointment, schedule })
        //        .Where(joined => 
        //            startDateTime <= joined.appointment.AppointmentDate
        //            && joined.appointment.AppointmentDate <= endDateTime
        //            && joined.schedule.DoctorId == doctorId)
        //        .Select(joined => joined.appointment)
        //        .OrderBy(app => app.AppointmentDate)
        //        .ThenBy(app => app.AppointmentTime)
        //        .ToListAsync();
        //}

        public async Task Book(Appointment appointment)
        {
            await repository.AddAsync(appointment);
            await repository.SaveChangesAsync<Appointment>();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<AppointmentDto>> GetAllByDate(string dateStr, CancellationToken ct)
        {
            try
            {
                var date = DateOnly.ParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                return await repository.AllReadOnly<Appointment>()
                    .Where(a => a.AppointmentDate == date)
                    .ProjectTo<AppointmentDto>(mapper.ConfigurationProvider)
                    .OrderBy(a => a.AppointmentTime)
                    .ToListAsync(ct);
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Date '{dateStr}' is not in the expected format 'yyyy-MM-dd'.", ex);
            }
        }

        public async Task Cancel(string appointmentId)
        {
            await repository.DeleteAsync<Appointment>(appointmentId);
            await repository.SaveChangesAsync<Appointment>();
        }
    }
}
