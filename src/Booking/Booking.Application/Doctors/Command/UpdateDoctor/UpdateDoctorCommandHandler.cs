using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

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
            var doctor = await _dbContext.Doctors
                .FindAsync([request.UserId], cancellationToken) ??
                throw new KeyNotFoundException("Doctor not found.");

            var currentDoctorId = _currentUser.UserId;

            if (string.IsNullOrEmpty(currentDoctorId))
                throw new UnauthorizedAccessException();

            var isOwner = doctor.UserId == currentDoctorId;
            var currentUser = await _userManager.FindByIdAsync(currentDoctorId);
            var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, Roles.Admin);

            if (!isOwner && isAdmin)
                throw new UnauthorizedAccessException("You can only edit your own profile.");

            doctor.UpdateProfile(
                request.Name,
                request.Lastname,
                request.Bio,
                request.ExperienceYears,
                request.ImageUrl,
                request.IsActive,
                request.ConsultationFee);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
