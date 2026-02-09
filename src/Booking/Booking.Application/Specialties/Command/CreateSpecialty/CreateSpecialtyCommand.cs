using MediatR;

namespace Booking.Application.Specialties.Command.CreateSpecialty
{
    public record CreateSpecialtyCommand (string Name)
        : IRequest<Guid>;
}
