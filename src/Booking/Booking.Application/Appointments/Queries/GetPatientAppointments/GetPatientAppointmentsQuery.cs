using Booking.Application.Appointments.Dtos;
using MediatR;

namespace Booking.Application.Appointments.Queries.GetPatientAppointments
{
    public record GetPatientAppointmentsQuery
        : IRequest<List<AppointmentDto>>;
}
