using Booking.Application.Doctors.Dtos;
using MediatR;

namespace Booking.Application.Doctors.Queries.GetDoctorStats
{
    public record GetDoctorStatsQuery(Guid DoctorId, string Period) 
        : IRequest<DoctorStatsDto>;
}
