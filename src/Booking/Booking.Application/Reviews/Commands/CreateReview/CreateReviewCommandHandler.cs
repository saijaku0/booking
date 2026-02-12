using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler(
        IBookingDbContext context,
        ICurrentUserService currentUser)
        : IRequestHandler<CreateReviewCommand, Guid>
    {
        private readonly IBookingDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUser;
        public async Task<Guid> Handle(
            CreateReviewCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not authorized to create reviews.");

            var patientId = await _context.Patients
                .AsNoTracking()
                .Where(p => p.ApplicationUserId == userId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (patientId == Guid.Empty)
                throw new UnauthorizedAccessException("Patient profile not found.");

            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.Id == request.DoctorId, cancellationToken);

            if (!doctorExists)
                throw new KeyNotFoundException($"Doctor with ID {request.DoctorId} not found.");

            var review = new Review(
                request.DoctorId,
                patientId,
                request.Rating,
                request.Text
            );

            _context.Reviews.Add(review);

            await _context.SaveChangesAsync(cancellationToken);

            return review.ReviewId;
        }
    }
}
