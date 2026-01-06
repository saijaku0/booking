namespace Booking.Application.Appointments.Dtos
{
    public class DoctorDto
    {
        public string? UserId { get; init; } = null;
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
    }
}
