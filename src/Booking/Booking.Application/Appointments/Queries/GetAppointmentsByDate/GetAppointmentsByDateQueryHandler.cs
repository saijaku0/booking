using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Extension;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetAppointmentsByDate
{
    public class GetAppointmentsByDateQueryHandler(IBookingDbContext context) 
        : IRequestHandler<GetAppointmentsByDateQuery, List<AppointmentDto>>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<List<AppointmentDto>> Handle(GetAppointmentsByDateQuery request, CancellationToken cancellationToken)
        {
            var getDateAppoinment = await _context.Appointments
                .Where(a => a.Status != AppointmentStatus.Canceled)
                .WhereOverlaps(request.DoctorId,
                    request.StartTime,
                    request.EndTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    CustomerId = a.CustomerId,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                })
                .ToListAsync(cancellationToken);

            return getDateAppoinment;
        }
    }
}
