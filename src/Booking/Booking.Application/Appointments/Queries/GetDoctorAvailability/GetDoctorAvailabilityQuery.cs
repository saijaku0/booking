using Booking.Application.Appointments.Dtos;
using MediatR;

namespace Booking.Application.Appointments.Queries.GetDoctorAvailability
{
    public record GetDoctorAvailabilityQuery(
        Guid DoctorId, 
        DateTime Date)
        : IRequest<List<TimeSlotDto>>;
}
