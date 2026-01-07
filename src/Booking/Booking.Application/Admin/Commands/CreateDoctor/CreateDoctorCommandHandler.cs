using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Booking.Application.Admin.Commands.CreateDoctor
{
    public class CreateDoctorCommandHandler (
        IBookingDbContext dbContext, 
        UserManager<ApplicationUser> userManager) 
        : IRequestHandler<CreateDoctorCommand, Guid>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Guid> Handle(
            CreateDoctorCommand createDoctor, 
            CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = createDoctor.Email,
                Email = createDoctor.Email,
                FirstName = createDoctor.Name,
                LastName = createDoctor.Lastname,
                EmailConfirmed = true,
            };

            var createUser = await _userManager
                .CreateAsync(user, createDoctor.Password);

            if (!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(e =>
                    new FluentValidation.Results.ValidationFailure("CreateUser", e.Description));

                throw new ValidationException(errors);
            }

            var roleResult = await _userManager
                .AddToRoleAsync(user, Roles.Doctor);

            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(e =>
                    new FluentValidation.Results.ValidationFailure("AddRole", e.Description));
                throw new ValidationException(errors);
            }

            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = createDoctor.Name,
                Lastname = createDoctor.Lastname,
                Specialty = createDoctor.Specialty,
            };

            _dbContext.Doctors.Add(doctor);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return doctor.Id;
        }
    }
}
