using Booking.Application.Appointments.Dtos;
using MediatR;

namespace Booking.Application.Appointments.Queries.GetAppointmentsByDate
{
    public record GetAppointmentsByDateQuery : IRequest<List<AppointmentDto>>
    {
        public Guid ResourceId { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
    }
}
