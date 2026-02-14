using Booking.Application.Specialties.Dtos;
using MediatR;

namespace Booking.Application.Specialties.Queries.GetListSpecialties
{
    public record GetSpecialtiesQuery 
        : IRequest<List<SpecialtyDto>>;
}
