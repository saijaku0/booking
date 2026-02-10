namespace Booking.Application.Doctors.Dtos
{
    public record DoctorDto(
        Guid Id,
        string? UserId,
        string Name,
        string Lastname,
        Guid SpecialtyId,
        string? ImageUrl,
        double AverageRating,
        int ReviewCount,
        string SpecialtyName,
        decimal ConsultationFee,
        int ExperienceYears,
        string? Bio);
}
