namespace Booking.Domain.Entities
{
    public class Specialty
    {
        public Guid SpecialtyId { get; private set; }
        public string Name { get; private set; } = null!;
        public DateTime DateTimeCreated { get; private set; }
        public DateTime? DateTimeUpdated { get; private set; }

        private Specialty() { }

        public Specialty(string name)
        {
            SpecialtyId = Guid.NewGuid();
            Name = name;
            DateTimeCreated = DateTime.UtcNow;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specialty name cannot be empty.", nameof(name));

            Name = name;
            DateTimeUpdated = DateTime.UtcNow;
        }
    }
}
