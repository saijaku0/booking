using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Specialties.Dtos;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Specialties.Queries.GetSpecialtiesById
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
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SpecialtyId == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Specialty), request.Id);

            return new SpecialtyDto
            {
                Id = entity.SpecialtyId,
                Name = entity.Name
            };
        }
    }
}
