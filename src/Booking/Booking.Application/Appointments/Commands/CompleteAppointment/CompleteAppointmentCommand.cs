using MediatR;

namespace Booking.Application.Appointments.Commands.CompleteAppointment
{
    public record CompleteAppointmentCommand(
        Guid AppointmentId,
        string MedicalNotes
        ) : IRequest;
}
