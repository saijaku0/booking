using Booking.Application.Common.Interfaces;
using Booking.Application.Specialties.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Specialties.Queries
{
    public class GetSpecialtiesQueryHandler(IBookingDbContext dbContext) 
        : IRequestHandler<GetSpecialtiesQuery, List<SpecialtyDto>>
    {
        private readonly IBookingDbContext _dbContext = dbContext;

        public async Task<List<SpecialtyDto>> Handle(
            GetSpecialtiesQuery request, 
            CancellationToken cancellationToken)
        {
            var specialties = await _dbContext.Specialties
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new SpecialtyDto
                {
                    Id = s.SpecialtyId,
                    Name = s.Name
                })
                .ToListAsync(cancellationToken);
            return specialties;
        }
    }
}
