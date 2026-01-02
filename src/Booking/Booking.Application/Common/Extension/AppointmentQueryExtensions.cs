using Booking.Domain.Entities;

namespace Booking.Application.Common.Extension
{
    public static class AppointmentQueryExtensions
    {
        public static IQueryable<Appointment> WhereOverlaps(
        this IQueryable<Appointment> query,
        Guid resourceId,
        DateTime start,
        DateTime end)
        {
            return query.Where(x => 
                x.ResourceId == resourceId
                    &&
                start < x.EndTime
                    &&
                end > x.StartTime);
        }
    }
}
