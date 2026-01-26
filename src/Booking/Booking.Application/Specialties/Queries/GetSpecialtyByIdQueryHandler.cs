using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Specialties.Dtos;
using Booking.Domain.Entities;
using MediatR;

namespace Booking.Application.Specialties.Queries
{
    public class GetSpecialtyByIdQueryHandler(IBookingDbContext context)
    : IRequestHandler<GetSpecialtyByIdQuery, SpecialtyDto>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<SpecialtyDto> Handle(
            GetSpecialtyByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _context.Specialties
                .FindAsync([request.Id], cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Specialty), request.Id);
            }

            return new SpecialtyDto
            {
                Id = entity.SpecialtyId,
                Name = entity.Name
            };
        }
    }
}
