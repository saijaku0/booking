using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Booking.Application.Specialties.Command.RemoveSpecialty
{
    public class RemoveSpecialtyCommandHandler(IBookingDbContext context)
        : IRequestHandler<RemoveSpecialtyCommand>
    {
        private readonly IBookingDbContext _context = context;

        public async Task Handle(RemoveSpecialtyCommand request, 
            CancellationToken cancellationToken)
        {
            var entity = await _context.Specialties
                .FindAsync([request.Id], cancellationToken)
                ?? throw new NotFoundException("Specialty", request.Id);
            

            var hasDoctors = await _context.Doctors
                .AnyAsync(d => d.SpecialtyId == request.Id, cancellationToken);

            if (hasDoctors)
            {
                throw new InvalidOperationException("Cannot delete specialty because it is assigned to one or more doctors.");
            }

            _context.Specialties.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
