using Booking.Application.Appointments.Dtos;
using MediatR;

namespace Booking.Application.Appointments.Queries.GetAppointmentsByDate
{
    public record GetAppointmentsByDateQuery : IRequest<List<AppointmentListDto>>
    {
        public Guid DoctorId { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
    }
}
