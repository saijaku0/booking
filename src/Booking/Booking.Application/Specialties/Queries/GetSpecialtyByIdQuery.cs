using Booking.Application.Specialties.Dtos;
using MediatR;

namespace Booking.Application.Specialties.Queries
{
    public record GetSpecialtyByIdQuery(Guid Id)
        : IRequest<SpecialtyDto>;
}
