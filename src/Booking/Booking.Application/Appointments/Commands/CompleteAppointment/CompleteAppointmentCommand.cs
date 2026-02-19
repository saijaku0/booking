using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Appointments.Commands.CompleteAppointment
{
    [Authorize(Roles = [Roles.Doctor])]
    public record CompleteAppointmentCommand(
        Guid AppointmentId,
        string Diagnosis,
        string? MedicalNotes,
        string? TreatmentPlan,
        string? PrescribedMedications
        ) : IRequest;
}
