using Booking.Application.Specialties.Dtos;
using MediatR;

namespace Booking.Application.Specialties.Queries
{
    public record GetSpecialtiesQuery 
        : IRequest<List<SpecialtyDto>>;
}
