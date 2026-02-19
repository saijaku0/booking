using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Appointments.Commands.CreateAppointment
{
    [Authorize(Roles = [Roles.Patient])]
    public record CreateAppointmentCommand(
            Guid DoctorId,
            DateTime StartTime,
            DateTime EndTime
        ) : IRequest<Guid>;
}
