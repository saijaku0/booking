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
            var doctorId = await GetCurrentDoctorIdAsync(cancellationToken);

            var query = _context.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorId == doctorId);

            if (request.Start.HasValue)
                query = query.Where(a => a.StartTime >= request.Start.Value);

            if (request.End.HasValue)
                query = query.Where(a => a.EndTime <= request.End.Value);

            return await query
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
        }

        private async Task<Guid> GetCurrentDoctorIdAsync(CancellationToken token)
        {
            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId))
                throw new UnauthorizedAccessException("User is not logged in.");

            var doctorId = await _context.Doctors
                .Where(d => d.UserId == currentUserId)
                .Select(d => d.Id)
                .FirstOrDefaultAsync(token);

            if (doctorId == Guid.Empty) 
                throw new UnauthorizedAccessException("User is not a registered doctor.");

            return doctorId;
        }
    }
}
