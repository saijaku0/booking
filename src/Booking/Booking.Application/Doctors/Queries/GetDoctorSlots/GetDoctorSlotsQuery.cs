using Booking.Application.Doctors.Dtos;
using MediatR;

namespace Booking.Application.Doctors.Queries.GetDoctorSlots
{
    public record GetDoctorSlotsQuery(Guid DoctorId,
        DateTime Date
    ) : IRequest<List<DoctorTimeSlotDto>>;
}
