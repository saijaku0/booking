using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;

namespace Booking.Application.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler(IBookingDbContext context) 
        : IRequestHandler<CreateReviewCommand, Guid>
    {
        private readonly IBookingDbContext _context = context;
        public async Task<Guid> Handle(
            CreateReviewCommand request, 
            CancellationToken cancellationToken)
        {
            var doctor = await _context.Doctors
                .FindAsync(
                    [ request.DoctorId ],
                    cancellationToken);

            if (doctor == null) 
                throw new ArgumentException("DoctorId cannot be empty.");

            var review = new Review
            (
                request.DoctorId,
                request.Rating
            );
            _context.Reviews.Add(review);
            doctor.AddReview(request.Rating);
            await _context.SaveChangesAsync(cancellationToken);
            return review.ReviewId;
        }
    }
}
