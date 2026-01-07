using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using Booking.Domain.Constants;

namespace Booking.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserCommandHandler(UserManager<ApplicationUser> user) 
        : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _user = user;

        public async Task<string> Handle(
            RegisterUserCommand request, 
            CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                FirstName = request.UserName,
                LastName = request.UserSurname,
                Email = request.UserEmail,
                UserName = request.UserEmail,
            };

            var createUser = await _user.CreateAsync(user, request.UserPassword);

            if (!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(e => 
                    new FluentValidation.Results.ValidationFailure("Registration", e.Description));

                throw new ValidationException(errors);
            }

            var roleResult = await _user.AddToRoleAsync(user, Roles.Patient);

            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(e =>
                    new FluentValidation.Results.ValidationFailure("RoleAssignment", e.Description));

                throw new ValidationException(errors);
            }

            return user.Id;
        }
    }
}
