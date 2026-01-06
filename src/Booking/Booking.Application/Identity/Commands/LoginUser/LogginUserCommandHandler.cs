using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Booking.Application.Identity.Commands.LoginUser;

public class LoginUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService)
    : IRequestHandler<LoginUserCommand, string>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            throw new ValidationException(
            [
                new ValidationFailure("Login", "Invalid email")
            ]);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("Login", "Invalid password")
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var token = _tokenService.GenerateToken(user, roles);

        return token;
    }
}