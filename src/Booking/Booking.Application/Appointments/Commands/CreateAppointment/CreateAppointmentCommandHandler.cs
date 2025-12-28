using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;

namespace Booking.Application.Appointments.Commands.CreateAppointment
{
    public class CreateAppointmentCommandHandler(IBookingDbContext context) : IRequestHandler<CreateAppointmentCommand, Guid>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            Appointment appointment = new(
                request.CustomerId,
                request.ResourceId,
                request.StartTime,
                request.EndTime
            );

            _context.Appointments.Add( appointment );

            await _context.SaveChangesAsync(cancellationToken);

            return appointment.Id;
        }
    }
}
