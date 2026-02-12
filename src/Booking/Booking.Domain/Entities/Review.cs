namespace Booking.Domain.Entities
{
    public class Review
    {
        public Guid ReviewId { get; private set; }
        public Guid PatientId { get; private set; }
        public virtual Patient Patient { get; private set; } = null!;
        public Guid DoctorId { get; private set; }
        public virtual Doctor Doctor { get; private set; } = null!;
        public int Rating { get; private set; }
        public string Text { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private Review() { }

        public Review(Guid doctorId, 
            Guid patientId, 
            int rating, 
            string text)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty.", nameof(text));

            ReviewId = Guid.NewGuid();
            DoctorId = doctorId;
            PatientId = patientId;
            Rating = rating;
            Text = text;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(int newRating, string newText)
        {
            if (newRating < 1 || newRating > 5) throw new ArgumentOutOfRangeException(nameof(newRating));

            Rating = newRating;
            Text = newText;
        }
    }
}
