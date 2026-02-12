namespace Booking.API.Dtos.Doctor
{
    public record UpdateScheduleRequest(
        TimeSpan DayStart,
        TimeSpan DayEnd,
        TimeSpan LunchStart,
        TimeSpan LunchEnd,
        int[] WorkingDays,
        int SlotDurationMinutes,
        int BufferMinutes
    );
}
