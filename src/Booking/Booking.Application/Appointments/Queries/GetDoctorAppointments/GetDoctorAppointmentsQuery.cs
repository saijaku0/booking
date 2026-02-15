using Booking.Application.Appointments.Dtos;
using MediatR;

namespace Booking.Application.Appointments.Queries.GetDoctorAppointments
{
    public record GetDoctorAppointmentsQuery
        : IRequest<List<AppointmentDetailDto>>
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
