using MediatR;

namespace Booking.Application.Appointments.Commands.CreateAppointment
{
    public record CreateAppointmentCommand(
            Guid CustomerId,
            Guid ResourceId,
            DateTime StartTime,
            DateTime EndTime
        ) : IRequest<Guid>;
}
