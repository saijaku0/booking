
using MediatR;

namespace Booking.Application.Identity.Commands.RegisterUser
{
    public record RegisterUserCommand : IRequest<string>
    {
        public string UserName { get; set; } = string.Empty;
        public string UserSurname { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}
