namespace Booking.Application.Appointments.Dtos
{
    /// <summary>
    /// Reservation information to display to the client
    /// </summary>
    public record AppointmentDto
    {
        /// <summary>
        /// Unique booking identifier
        /// </summary>
        public Guid Id { get; init; }
        /// <summary>
        /// Resource ID(meeting room/office)
        /// </summary>
        public Guid ResourceId { get; init; }
        /// <summary>
        /// ID of the client who made the reservation
        /// </summary>
        public Guid CustomerId { get; init; }
        /// <summary>
        /// Start date and time (UTC)
        /// </summary>
        public DateTime StartTime { get; init; }
        /// <summary>
        /// End date and time (UTC)
        /// </summary>
        public DateTime EndTime { get; init; }
    }
}
