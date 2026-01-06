using Booking.Domain.Entities;

namespace Booking.Application.Common.Extension
{
    public static class AppointmentQueryExtensions
    {
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
