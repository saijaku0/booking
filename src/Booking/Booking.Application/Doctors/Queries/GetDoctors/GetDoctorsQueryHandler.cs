using Booking.Application.Common.Interfaces;
using Booking.Application.Doctors.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Doctors.Queries.GetDoctors
{
    public class GetDoctorsQueryHandler(IBookingDbContext context) 
        : IRequestHandler<GetDoctorsQuery, List<DoctorDto>>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<List<DoctorDto>> Handle(
            GetDoctorsQuery getDoctorsQuery, 
            CancellationToken cancellationToken)
        {
            var query = _context.Doctors
            .AsNoTracking() 
            .AsQueryable();

            if (getDoctorsQuery.SpecialtyId.HasValue)
            {
                query = query.Where(d => d.SpecialtyId == getDoctorsQuery.SpecialtyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(getDoctorsQuery.SearchTerm))
            {
                var term = getDoctorsQuery.SearchTerm.ToLower().Trim();

                query = query.Where(d =>
                    d.Name.ToLower().Contains(term) ||
                    d.Lastname.ToLower().Contains(term));
            }

            query = query
                .Where(d => d.IsActive) 
                .OrderByDescending(d => d.AverageRating)
                .ThenByDescending(d => d.ReviewsCount);

            return await query
                .Select(d => new DoctorDto
                (
                    d.Id,
                    d.Name,
                    d.Lastname,
                    d.SpecialtyId,
                    d.ImageUrl,
                    d.AverageRating,
                    d.ReviewsCount,
                    d.Specialty != null ? d.Specialty.Name : "General",
                    d.ConsultationFee,
                    d.ExperienceYears
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
