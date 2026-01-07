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

    public async Task<string> Handle(
        LoginUserCommand request, 
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        bool isPasswordValid = user != null 
            && await _userManager.CheckPasswordAsync(user, request.Password);

        if (user == null || !isPasswordValid)
        {
            throw new ValidationException(
            [
                new ValidationFailure("Login", "Invalid email or password")
            ]);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return token;
    }
}