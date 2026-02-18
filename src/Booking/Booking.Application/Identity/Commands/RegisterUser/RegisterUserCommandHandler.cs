using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IBookingDbContext dbContext) 
        : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IBookingDbContext _dbContext = dbContext;

        public async Task<string> Handle(
            RegisterUserCommand request, 
            CancellationToken cancellationToken)
        {
            var user = ApplicationUser.CreatePatient
            (
                firstName: request.UserName,
                lastName: request.UserSurname,
                email: request.UserEmail,
                phoneNumber: request.PhoneNumber ?? string.Empty,
                address: request.Address ?? string.Empty
            );

            var createResult = await _userManager.CreateAsync(user, request.UserPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Patient);
            if (!roleResult.Succeeded) throw new Exception("Failed to assign role");

            var patient = new Patient(
                user.Id,                
                request.DateOfBirth,
                request.Gender,
                request.PhoneNumber,
                request.Address
            );

            _dbContext.Patients.Add(patient);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
