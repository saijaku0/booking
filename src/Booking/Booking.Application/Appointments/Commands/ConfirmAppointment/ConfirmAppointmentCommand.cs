using MediatR;

namespace Booking.Application.Appointments.Commands.ConfirmAppointment
{
    public record ConfirmAppointmentCommand(Guid AppointmentId) 
        : IRequest<Unit>;
}
