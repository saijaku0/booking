using Booking.Application.Common.Interfaces;
using Booking.Application.Reviews.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Reviews.Query
{
    public class GetDoctorReviewsQueryHandler(
        IBookingDbContext context,
        IIdentityService identityService)
        : IRequestHandler<GetDoctorReviewsQuery, List<ReviewDto>>
    {
        private readonly IBookingDbContext _context = context;
        private readonly IIdentityService _identityService = identityService;
        public async Task<List<ReviewDto>> Handle(
            GetDoctorReviewsQuery request,
            CancellationToken cancellationToken)
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.DoctorId == request.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            var res = new List<ReviewDto>();

            foreach (var r in reviews) {
                var patient = await _identityService.GetUserNameAsync(r.PatientId.ToString())
                    ?? "Anonymous";
                res.Add(new ReviewDto(
                    r.ReviewId,
                    patient,
                    r.Rating,
                    r.Text,
                    r.CreatedAt
                ));
            }

            return res;
        }
    }
}
