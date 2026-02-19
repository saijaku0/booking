using Booking.Application.Common.Security;
using Booking.Application.Doctors.Dtos;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Doctors.Queries.GetDoctorStats
{
    [Authorize(Roles = [Roles.Doctor, Roles.Admin])]
    public record GetDoctorStatsQuery(Guid DoctorId, string Period) 
        : IRequest<DoctorStatsDto>;
}
