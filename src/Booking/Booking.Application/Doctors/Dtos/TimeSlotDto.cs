namespace Booking.Application.Doctors.Dtos
{
    public class TimeSlotDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAvailable { get; set; }
    }
}
