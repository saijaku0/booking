namespace Booking.Domain.Entities
{
    public class Doctor
    {
        public Guid Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
    }
}
