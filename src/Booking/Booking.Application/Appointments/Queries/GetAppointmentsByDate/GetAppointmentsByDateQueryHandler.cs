using Booking.Application.Appointments.Queries.GetAppointmentById;
using Booking.Application.Common.Interfaces;
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
                .Where(x =>
                    x.ResourceId == request.ResourceId
                        &&
                    request.StartTime < x.EndTime
                        &&
                    request.EndTime > x.StartTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    ResourceId = a.ResourceId,
                    CustomerId = a.CustomerId,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                })
                .ToListAsync(cancellationToken);

            return getDateAppoinment;
        }
    }
}
