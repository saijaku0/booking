using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using FluentValidation;

namespace Booking.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserCommandHandler(UserManager<ApplicationUser> user) 
        : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _user = user;

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                FirstName = request.UserName,
                LastName = request.UserSurname,
                Email = request.UserEmail,
                UserName = request.UserEmail,
            };

            var result = await _user.CreateAsync(user, request.UserPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => 
                    new FluentValidation.Results.ValidationFailure("Registration", e.Description));

                throw new ValidationException(errors);
            }

                return user.Id;
        }
    }
}
