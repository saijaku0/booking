using MediatR;

namespace Booking.Application.Appointments.Commands.CompleteAppointment
{
    public record CompleteAppointmentCommand(
        Guid AppointmentId,
        string Diagnosis,
        string? MedicalNotes,
        string? TreatmentPlan
        ) : IRequest;
}
