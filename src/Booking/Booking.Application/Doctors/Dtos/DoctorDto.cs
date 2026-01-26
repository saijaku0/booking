namespace Booking.Application.Doctors.Dtos
{
    public class DoctorDto
    {
        public Guid Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public Guid SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
    }
}
