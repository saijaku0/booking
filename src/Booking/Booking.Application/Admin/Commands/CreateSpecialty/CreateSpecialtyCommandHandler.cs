using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;

namespace Booking.Application.Admin.Commands.CreateSpecialty
{
    public class CreateSpecialtyCommandHandler(IBookingDbContext dbContext)
        : IRequestHandler<CreateSpecialtyCommand, Guid>
    {
        private readonly IBookingDbContext _dbContext = dbContext;

        public async Task<Guid> Handle(
            CreateSpecialtyCommand request, 
            CancellationToken cancellationToken)
        {
            var specialty = new Specialty
            (
                request.Name
            );
            _dbContext.Specialties.Add(specialty);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return specialty.SpecialtyId;
        }
    }
}
