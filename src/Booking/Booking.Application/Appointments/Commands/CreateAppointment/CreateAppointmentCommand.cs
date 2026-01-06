using MediatR;

namespace Booking.Application.Appointments.Commands.CreateAppointment
{
    public record CreateAppointmentCommand(
            Guid DoctorId,
            DateTime StartTime,
            DateTime EndTime
        ) : IRequest<Guid>;
}
