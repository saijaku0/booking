using MediatR;

namespace Booking.Application.Doctors.Command.CreateDoctor
{
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
