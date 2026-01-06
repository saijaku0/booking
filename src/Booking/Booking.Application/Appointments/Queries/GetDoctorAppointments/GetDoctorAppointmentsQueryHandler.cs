using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Booking.Application.Appointments.Queries.GetDoctorAppointments
{
    public class GetDoctorAppointmentsQueryHandler(
        IBookingDbContext bookingDbContext,
        ICurrentUserService currentUserService) 
        : IRequestHandler<GetDoctorAppointmentsQuery, List<AppointmentDto>>
    {
        private readonly IBookingDbContext _context = bookingDbContext;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<List<AppointmentDto>> Handle(
            GetDoctorAppointmentsQuery request, 
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(currentUserId))
                throw new UnauthorizedAccessException("User is not logged in.");

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == currentUserId, cancellationToken) ?? 
                throw new UnauthorizedAccessException("Current user is not a registered doctor.");

            var query = _context.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorId == doctor.Id);

            if (request.Start.HasValue)
                query = query.Where(a => a.StartTime >= request.Start.Value);

            if (request.End.HasValue)
                query = query.Where(a => a.EndTime <= request.End.Value);

            var appointments = await query
                .OrderBy(a => a.StartTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    CustomerId = a.CustomerId,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                })
                .ToListAsync(cancellationToken);

            return appointments;
        }
    }
}
