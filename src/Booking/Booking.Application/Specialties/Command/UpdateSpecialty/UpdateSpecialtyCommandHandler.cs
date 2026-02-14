using Booking.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Booking.Application.Specialties.Command.UpdateSpecialty
{
    public class UpdateSpecialtyCommandHandler(
        IBookingDbContext context) : IRequestHandler<UpdateSpecialtyCommand>
    {
        private readonly IBookingDbContext _context = context;
        public async Task Handle(
            UpdateSpecialtyCommand request, 
            CancellationToken cancellationToken)
        {
            var entity = await _context.Specialties
                .FindAsync([ request.Id ], cancellationToken)
                ?? throw new KeyNotFoundException($"Specialty with ID {request.Id} not found.");

            var checker = await _context.Specialties
                .AnyAsync(s => s.Name == request.Name 
                    && s.SpecialtyId != request.Id, cancellationToken);
            if (checker)
                throw new InvalidOperationException($"Specialty with name {request.Name} already exists.");

            entity.UpdateName(request.Name);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
