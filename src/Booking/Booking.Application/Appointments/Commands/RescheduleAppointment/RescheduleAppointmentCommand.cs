using MediatR;

namespace Booking.Application.Appointments.Commands.RescheduleAppointment
{
    public record RescheduleAppointmentCommand(
        Guid AppointmentId,
        DateTime NewStartTime,
        DateTime NewEndTime)
        : IRequest<Unit>
    {
    }
}
