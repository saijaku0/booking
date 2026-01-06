using Booking.Application.Appointments.Dtos;
using MediatR;

namespace Booking.Application.Appointments.Queries.GetDoctorAppointments
{
    public record GetDoctorAppointmentsQuery
        : IRequest<List<AppointmentDto>>
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
