using MediatR;

namespace Booking.Application.Reviews.Commands.CreateReview
{
    public record CreateReviewCommand(
        Guid DoctorId, 
        int Rating)
        : IRequest<Guid>; 
}
