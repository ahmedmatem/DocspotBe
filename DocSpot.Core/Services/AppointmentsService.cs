namespace DocSpot.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using DocSpot.Core.Contracts;
    using DocSpot.Core.Extensions;
    using DocSpot.Core.Models;
    using DocSpot.Core.Models.Req.Appointment;
    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.Infrastructure.Data.Repository;
    using DocSpot.Infrastructure.Data.Types;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using static DocSpot.Core.Constants;
    using static DocSpot.Core.Helpers.TimeHelper;
    using static DocSpot.Core.Helpers.TokenHelper;
    using static DocSpot.Core.Models.ActionResult;

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

        public async Task<string> Book(AppointmentDto appointmentDto, CancellationToken ct = default)
        {
            var appointment = mapper.Map<Appointment>(appointmentDto);
            appointment.Id = Guid.NewGuid().ToString();
            appointment.TokenExpireAtUtc = DateTime.UtcNow.AddHours(PublicTokenExpireHours);

            var appointmentStartUtc = GetAppointmentStartUtc(
                appointment.AppointmentDate,
                appointment.AppointmentTime);
            // set cancel token expiry CancelTokenExpireHours before appointment start time
            appointment.CancelTokenExpireAtUtc = appointmentStartUtc.AddHours(-CancelTokenExpireHours);

            await repository.AddAsync(appointment);
            await repository.SaveChangesAsync<Appointment>();

            return appointment.Id;
        }

        /// <summary>
        /// Gets all appointments scheduled for the specified date.
        /// </summary>
        /// <param name="dateStr">
        /// The date for which to retrieve appointments, in <c>yyyy-MM-dd</c> format.
        /// </param>
        /// <param name="ct">
        /// A cancellation token that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a read-only list of
        /// <see cref="AppointmentDto"/> instances scheduled on the specified date.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown if <paramref name="date"/> is not in the expected <c>yyyy-MM-dd</c> format.
        /// </exception>
        public async Task<IReadOnlyList<AppointmentDto>> GetAllByDateAsync(string dateStr, CancellationToken ct)
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

        /// <inheritdoc/>
        public async Task<AppointmentPublicDto?> GetCancelPreviewAsync(AppointmentPublicReq req, CancellationToken ct)
        {
            var appointment = await repository.All<Appointment>()
                .FirstOrDefaultAsync(a => a.Id == req.Id, ct);

            if (appointment is null) return null; // bad link

            var cancelHash = ComputeSha256Hash(req.Token);
            if (appointment.CancelTokenHash is null ||
                !cancelHash.Equals(appointment.CancelTokenHash, StringComparison.OrdinalIgnoreCase))
            {
                return null; // bad token
            }

            // 3 hours rule (before appointment start time)
            var deadlineUtc =
                GetCancelDeadlineUtc(appointment.AppointmentDate, appointment.AppointmentTime);
            if (DateTime.UtcNow > deadlineUtc)
            {
                return null; // too late to cancel
            }

            if (appointment.AppointmentStatus == AppointmentStatus.Cancelled)
            {
                return null; // already canceled
            }

            return mapper.Map<AppointmentPublicDto>(appointment);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> CancelAsync(AppointmentPublicReq req, CancellationToken ct)
        {
            var appointment = await repository.All<Appointment>()
                .FirstOrDefaultAsync(a => a.Id == req.Id, ct);

            if (appointment is null) return OperationResult.Failed; // bad link

            var cancelHash = ComputeSha256Hash(req.Token);
            if(appointment.CancelTokenHash is null ||
                !cancelHash.Equals(appointment.CancelTokenHash, StringComparison.OrdinalIgnoreCase))
            {
                return OperationResult.Failed; // bad token
            }

            // 3 hours rule (before appointment start time)
            var deadlineUtc = 
                GetCancelDeadlineUtc(appointment.AppointmentDate, appointment.AppointmentTime);
            if(DateTime.UtcNow > deadlineUtc)   
            {
                return OperationResult.Failed; // too late to cancel
            }

            if(appointment.AppointmentStatus == AppointmentStatus.Cancelled)
            {
                return OperationResult.Failed; // already canceled
            }

            appointment.AppointmentStatus = AppointmentStatus.Cancelled;
            appointment.CancelledAtUtc = DateTime.UtcNow;

            // invalidate the cancel token
            appointment.CancelTokenHash = null;
            appointment.CancelTokenExpireAtUtc = null;

            await repository.SaveChangesAsync<Appointment>(ct);

            return OperationResult.Success; // successfully canceled
        }

        public async Task<IReadOnlyCollection<AdminAppointmentDto>> GetList(
            AdminAppointmentsReq req, CancellationToken ct)
        {
            DateOnly? fromDate = req.From.ToDateOnlyOrNull();
            DateOnly? toDate = req.To.ToDateOnlyOrNull();

            var query = repository.AllReadOnly<Appointment>()
                .AsQueryable();

            if (fromDate is not null)
                query = query.Where(a => a.AppointmentDate >= fromDate);
            if (toDate is not null)
                query = query.Where(a => a.AppointmentDate <= toDate);
            if (req.Status is not null && req.Status == "CANCELLED")
            {
                query = query.Where(x => x.AppointmentStatus == AppointmentStatus.Cancelled);
            }
            else
            {
                query = query.Where(x => x.AppointmentStatus != AppointmentStatus.Cancelled);
            }
            if (!string.IsNullOrWhiteSpace(req.Query))
            {
                req.Query = req.Query.Trim();
                query = query.Where(x =>
                    x.PatientName.Contains(req.Query) ||
                    x.PatientPhone.Contains(req.Query) ||
                    x.PatientEmail.Contains(req.Query));
            }

            return await query
                .OrderBy(x => x.AppointmentDate)
                .ThenBy(x => x.AppointmentTime)
                .ProjectTo<AdminAppointmentDto>(mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }

        public async Task<AdminActionResult> AdminCancelAsync(string id, AdminAppointmentsActionReq.CancelAppointmentReq req, CancellationToken ct)
        {
            //var appt = await repository.AllReadOnly<Appointment>()
            //    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
            //if (appt is null) return AdminActionResult.NotFound;

            //if (appt.AppointmentStatus == AppointmentStatus.Cancelled)
            //    return AdminActionResult.Success;

            //appt.AppointmentStatus = AppointmentStatus.Cancelled;
            //appt.CancelReason = req.Reason?.Trim();
            //appt.CancelledAt = DateTime.UtcNow;
            //appt.CancelledByAdminId = _current.UserId;

            //await _db.SaveChangesAsync(ct);

            //if (req.NotifyPatient && !string.IsNullOrWhiteSpace(appt.PatientEmail))
            //{
            //    await _email.SendAppointmentCancelledAsync(
            //        appt.PatientEmail,
            //        appt.PatientName,
            //        appt.AppointmentDate,
            //        appt.AppointmentTime,
            //        appt.CancelReason,
            //        ct);
            //}

            return AdminActionResult.Success;
        }
    }
}
