using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Doctors.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Doctors.Queries.GetDoctorById
{
    public class GetDoctorByIdQueryHandler(
        IBookingDbContext dbContext)
        : IRequestHandler<GetDoctorByIdQuery, DoctorDto>
    {
        private readonly IBookingDbContext _dbContext = dbContext;

        public async Task<DoctorDto> Handle(
            GetDoctorByIdQuery request,
            CancellationToken cancellationToken)
        {
            var doctor = await _dbContext.Doctors
                .AsNoTracking()
                .Where(d => d.Id == request.Id)
                .Select(s => new DoctorDto(
                    s.Id,
                    s.Name,
                    s.Lastname,
                    s.SpecialtyId,
                    s.ImageUrl,
                    s.Reviews.Any() ? s.Reviews.Average(r => r.Rating) : 0.0,
                    s.Reviews.Count(),
                    s.Specialty != null ? s.Specialty.Name : "No Specialty",
                    s.ConsultationFee,
                    s.ExperienceYears
                    ))
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Doctor", request.Id);

            return doctor;
        }
    }
}
