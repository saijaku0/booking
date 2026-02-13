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

            var userFirstName = patient.ApplicationUser?.FirstName ?? "Unknown";
            var userLastName = patient.ApplicationUser?.LastName ?? "User";
            var userEmail = patient.ApplicationUser?.Email ?? string.Empty;
            var userPhoto = patient.ApplicationUser?.PhotoUrl;

            return new PatientProfileDto
            {
                Id = patient.Id,
                FullName = $"{userFirstName} {userLastName}",
                Email = userEmail,
                PhotoUrl = userPhoto,
                PhoneNumber = patient.PhoneNumber,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender.ToString(),
                Address = patient.Address
            };
        }
    }
}
