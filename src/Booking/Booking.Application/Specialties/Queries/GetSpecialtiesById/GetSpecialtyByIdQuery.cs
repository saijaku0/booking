using Booking.Application.Specialties.Dtos;
using MediatR;

namespace Booking.Application.Specialties.Queries.GetSpecialtiesById
{
    public record GetSpecialtyByIdQuery(Guid Id)
        : IRequest<SpecialtyDto>;
}
