using MediatR;

namespace Booking.Application.Admin.Commands.CreateSpecialty
{
    public record CreateSpecialtyCommand (string Name)
        : IRequest<Guid>;
}
