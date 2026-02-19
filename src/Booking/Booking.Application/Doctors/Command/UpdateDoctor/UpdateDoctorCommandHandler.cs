using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Doctors.Command.UpdateDoctor
{
    public class UpdateDoctorCommandHandler(
        IBookingDbContext dbContext,
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager) 
        : IRequestHandler<UpdateDoctorCommand, Unit>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly ICurrentUserService _currentUser = currentUser;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Unit> Handle(
            UpdateDoctorCommand request, 
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            if (string.IsNullOrEmpty(currentUserId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var doctor = await _dbContext.Doctors
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.Id == request.UserId, cancellationToken) 
                ?? throw new KeyNotFoundException("Doctor not found.");

            var currentDoctorId = _currentUser.UserId;

            if (string.IsNullOrEmpty(currentDoctorId))
                throw new UnauthorizedAccessException();

            var isOwner = doctor.ApplicationUserId == currentDoctorId;
            var currentUser = await _userManager.FindByIdAsync(currentDoctorId);
            var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, Roles.Admin);

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException("You can only edit your own profile.");

            if (doctor.ApplicationUser == null)
                throw new InvalidOperationException($"Doctor with id {doctor.Id} has no associated user.");

            doctor.UpdateProfile(
                request.Bio,
                request.ExperienceYears,
                request.ImageUrl,
                request.IsActive,
                request.ConsultationFee);

            doctor.ApplicationUser.UpdatePersonalInfo(
                request.Name,
                request.Lastname,
                request.PhoneNumber);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
