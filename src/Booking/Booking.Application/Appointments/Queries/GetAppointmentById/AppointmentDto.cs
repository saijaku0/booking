namespace Booking.Application.Appointments.Queries.GetAppointmentById
{
    public record AppointmentDto
    {
        public Guid Id { get; init; }
        public Guid ResourceId { get; init; }
        public Guid CustomerId { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
    }
}
