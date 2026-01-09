namespace Booking.Application.Doctor.Dtos
{
    public class DoctorDto
    {
        public Guid Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
    }
}
