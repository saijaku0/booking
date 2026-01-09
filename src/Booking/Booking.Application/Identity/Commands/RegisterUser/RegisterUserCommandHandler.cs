using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Booking.Domain.Constants;
using Booking.Application.Common.Exceptions;

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

            (await _user.CreateAsync(user, request.UserPassword))
                .EnsureSucceeded("Registration");

            (await _user.AddToRoleAsync(user, Roles.Patient))
                .EnsureSucceeded("RoleAssignment");

            return user.Id;
        }
    }
}
