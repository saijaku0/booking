
using MediatR;

namespace Booking.Application.Identity.Commands.RegisterUser
{
    public record RegisterUserCommand(
    string UserName,
    string UserSurname,
    string UserEmail,
    string UserPassword,
    DateOnly DateOfBirth,
    Gender Gender,
    string? PhoneNumber
) : IRequest<string>;
}
