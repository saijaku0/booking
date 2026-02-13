using MediatR;

namespace Booking.Application.Doctors.Command.UpdateDoctor
{
    public record UpdateDoctorCommand(
        Guid UserId,
        string Name,
        string Lastname,
        string Specialty,
        string PhoneNumber,
        string Bio,
        int ExperienceYears,
        string ImageUrl,
        decimal ConsultationFee,
        bool IsActive) : IRequest<Unit>;
}
