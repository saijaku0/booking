using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Doctors.Command.CreateDoctor
{
    [Authorize(Roles = [Roles.Admin])]
    public record CreateDoctorCommand(
        string Email,
        string Password,
        string Name,
        string Lastname,
        string PhoneNumber,
        Guid SpecialtyId,
        bool IsActive,
        decimal ConsultationFee,
        int ExperienceYears,
        string? Bio,
        string? ImageUrl
    ) : IRequest<Guid>;
}
