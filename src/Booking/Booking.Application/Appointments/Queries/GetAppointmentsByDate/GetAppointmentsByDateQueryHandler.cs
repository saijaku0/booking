using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Booking.Application.Common.Extension;

namespace Booking.Application.Appointments.Queries.GetAppointmentsByDate
{
    public class GetAppointmentsByDateQueryHandler(IBookingDbContext context) 
        : IRequestHandler<GetAppointmentsByDateQuery, List<AppointmentDto>>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<List<AppointmentDto>> Handle(GetAppointmentsByDateQuery request, CancellationToken cancellationToken)
        {
            var getDateAppoinment = await _context.Appointments
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
