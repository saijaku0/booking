using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Appointments.Commands.RescheduleAppointment
{
    [Authorize(Roles = [Roles.Doctor, Roles.Admin])]
    public record RescheduleAppointmentCommand(
        Guid AppointmentId,
        DateTime NewStartTime,
        DateTime NewEndTime)
        : IRequest<Unit>;
}
