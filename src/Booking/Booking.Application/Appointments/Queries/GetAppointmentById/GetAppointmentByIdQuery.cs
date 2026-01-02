using MediatR;

namespace Booking.Application.Appointments.Queries.GetAppointmentById
{
    public record GetAppointmentByIdQuery : IRequest<AppointmentDto>
    {
        public Guid Id { get; init; }
    }
}
