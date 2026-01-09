using MediatR;

namespace Booking.Application.Appointments.Commands.CancelAppointment
{
    public record CancelAppointmentCommand(
        Guid Id) : IRequest;
}
