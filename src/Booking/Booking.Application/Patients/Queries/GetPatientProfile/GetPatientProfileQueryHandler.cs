using Booking.Application.Common.Interfaces;
using Booking.Application.Patients.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Patients.Queries.GetPatientProfile
{
    public class GetPatientProfileQueryHandler(
        IBookingDbContext dbContext,
        ICurrentUserService currentUserService)
        : IRequestHandler<GetPatientProfileQuery, PatientProfileDto>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<PatientProfileDto> Handle(GetPatientProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not found.");

            var patient = await _dbContext.Patients
                .AsNoTracking()
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId, cancellationToken)
                ?? throw new UnauthorizedAccessException("Patient profile not found.");

            return new PatientProfileDto
            {
                Id = patient.Id,
                FullName = $"{patient.ApplicationUser!.FirstName} {patient.ApplicationUser.LastName}",
                Email = patient.ApplicationUser?.Email ?? string.Empty,
                PhotoUrl = patient.ApplicationUser?.PhotoUrl,
                PhoneNumber = patient.PhoneNumber,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender.ToString(),
                Address = patient.Address
            };
        }
    }
}
