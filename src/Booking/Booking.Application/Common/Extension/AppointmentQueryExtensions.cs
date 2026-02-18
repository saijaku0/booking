using Booking.Domain.Entities;

namespace Booking.Application.Common.Extension
{
    public static class AppointmentQueryExtensions
    {
        /// <summary>
        /// Filters appointments that overlap with the specified time interval.
        /// </summary>
        /// <param name="doctorId">Optional doctor ID to filter by. If null, all doctors are considered.</param>
        /// <param name="start">Start of the interval (exclusive on the right).</param>
        /// <param name="end">End of the interval (exclusive on the left).</param>
        /// <returns>An IQueryable of appointments that overlap with the interval.</returns>
        public static IQueryable<Appointment> WhereOverlaps(
        this IQueryable<Appointment> query,
        Guid? doctorId,
        DateTime start,
        DateTime end)
        {
            return query.Where(x => 
                (!doctorId.HasValue || x.DoctorId == doctorId)
                    &&
                start < x.EndTime
                    &&
                end > x.StartTime);
        }
    }
}
