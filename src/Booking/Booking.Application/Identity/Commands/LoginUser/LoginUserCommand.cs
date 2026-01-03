using MediatR;

namespace Booking.Application.Identity.Commands.LoginUser;

public record LoginUserCommand : IRequest<string>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}