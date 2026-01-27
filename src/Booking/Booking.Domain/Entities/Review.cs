namespace Booking.Domain.Entities
{
    public class Review
    {
        public Guid ReviewId { get; private set; }
        public Guid DoctorId { get; private set; }
        public Doctor Doctor { get; private set; } = null!;
        public int Rating { get; private set; }

        private Review() { }

        public Review(Guid doctorId, int rating)
        {
            ReviewId = Guid.NewGuid();
            DoctorId = doctorId;
            Rating = rating;
        }

        public void UpdateRating(int newRating)
        {
            if (newRating < 1 || newRating > 5)
                throw new ArgumentOutOfRangeException(nameof(newRating), "Rating must be between 1 and 5.");

            Rating = newRating;
        }
    }
}
