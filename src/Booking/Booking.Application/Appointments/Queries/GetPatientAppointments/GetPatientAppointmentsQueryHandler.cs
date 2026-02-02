using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Interfaces;
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

            return await _dbContext.Appointments
                .AsNoTracking()
                .Where(a => a.CustomerId == patientId) 
                .OrderByDescending(a => a.StartTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    CustomerId = a.CustomerId,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                })
                .ToListAsync(cancellationToken);
        }
    }
}
