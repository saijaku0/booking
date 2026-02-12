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
                .Include(r => r.Patient)
                .ThenInclude(p => p.ApplicationUser)
                .Where(r => r.DoctorId == request.Id)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto(

                    Id: r.ReviewId,
                    PatientName: r.Patient.ApplicationUser != null
                        ? $"{r.Patient.ApplicationUser.FirstName} {r.Patient.ApplicationUser.LastName}"
                        : "Anonymous", 
                    Rating: r.Rating,
                    Text: r.Text,
                    CreatedAt: r.CreatedAt
                    )
                )
                .ToListAsync(cancellationToken);

            return reviews;
        }
    }
}
