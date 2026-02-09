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
            var userIdString = _currentUser.UserId;

            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User is not authorized to create reviews.");

            if (!Guid.TryParse(userIdString, out var patientId))
                throw new UnauthorizedAccessException("Invalid User ID format.");

            var doctor = await _context.Doctors
                .Include(d => d.Reviews)
                .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken)
                    ?? throw new KeyNotFoundException($"Doctor with ID {request.DoctorId} not found.");

            var review = new Review(
                doctor.Id,
                patientId,
                request.Rating,
                request.Text
            );

            doctor.AddReview(review);

            await _context.SaveChangesAsync(cancellationToken);

            return review.ReviewId;
        }
    }
}
