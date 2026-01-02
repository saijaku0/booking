using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetAppointmentById
{
    public class GetAppointmentByIdQueryHandler(IBookingDbContext context) : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<AppointmentDto> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var appoinment = await _context.Appointments
                .Where(x => x.Id == request.Id)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    ResourceId = a.ResourceId,
                    CustomerId = a.CustomerId,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (appoinment == null) 
                throw new NotFoundException(nameof(appoinment), request.Id);

            return appoinment;
        }
    }
}
