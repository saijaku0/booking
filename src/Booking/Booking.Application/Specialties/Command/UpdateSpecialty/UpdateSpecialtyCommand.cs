using MediatR;

namespace Booking.Application.Specialties.Command.UpdateSpecialty
{
    public record UpdateSpecialtyCommand(Guid Id, string Name) 
        : IRequest;
}
