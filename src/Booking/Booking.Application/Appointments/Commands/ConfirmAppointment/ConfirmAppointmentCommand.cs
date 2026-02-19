using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Appointments.Commands.ConfirmAppointment
{
    [Authorize(Roles = [Roles.Doctor])]
    public record ConfirmAppointmentCommand(Guid AppointmentId) 
        : IRequest<Unit>;
}
