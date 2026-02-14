using MediatR;

namespace Booking.Application.Specialties.Command.RemoveSpecialty
{
    public record RemoveSpecialtyCommand(Guid Id) : IRequest;
}
