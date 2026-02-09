using Booking.Application.Reviews.Dtos;
using MediatR;

namespace Booking.Application.Reviews.Query
{
    public record GetDoctorReviewsQuery(Guid Id) 
        : IRequest<List<ReviewDto>>;
}
