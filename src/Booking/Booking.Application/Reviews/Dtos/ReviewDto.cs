namespace Booking.Application.Reviews.Dtos
{
    public record ReviewDto(
        Guid Id,
        string PatientName,
        int Rating,
        string text,
        DateTime CreatedAt);
}
