using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetPatientAppointments
{
    public class GetPatientAppointmentsQueryHandler(
        IBookingDbContext dbContext,
        ICurrentUserService userService)
        : IRequestHandler<GetPatientAppointmentsQuery, List<AppointmentDto>>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly ICurrentUserService _userService = userService;

        public async Task<List<AppointmentDto>> Handle(
            GetPatientAppointmentsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _userService.UserId;
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var patientId))
                throw new UnauthorizedAccessException("User is not authorized");

            var query = from appointment in _dbContext.Appointments
                join doctor in _dbContext.Doctors
                    on appointment.DoctorId equals doctor.Id
                join specialty in _dbContext.Specialties
                    on doctor.SpecialtyId equals specialty.SpecialtyId
                where appointment.CustomerId == patientId
                    orderby appointment.StartTime descending

                select new AppointmentDto
                {
                    Id = appointment.Id,
                    DoctorId = appointment.DoctorId,
                    CustomerId = appointment.CustomerId,
                    StartTime = appointment.StartTime,
                    EndTime = appointment.EndTime,
                    Status = appointment.Status.ToString(),
                    DoctorName = $"{doctor.ApplicationUser.FirstName} {doctor.ApplicationUser.LastName}",
                    Specialty = specialty.Name
                };

            return await query.ToListAsync(cancellationToken);
        }
    }
}
