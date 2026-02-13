namespace Booking.Application.Patients.Dtos
{
    public class PatientProfileDto
    {
        public Guid Id { get; set; } 
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string? PhoneNumber { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
    }
}
