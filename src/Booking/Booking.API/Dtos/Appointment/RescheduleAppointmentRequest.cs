namespace Booking.API.Dtos.Appointment
{
    public record RescheduleAppointmentRequest(
        DateTime StartTime, DateTime EndTime);
}
