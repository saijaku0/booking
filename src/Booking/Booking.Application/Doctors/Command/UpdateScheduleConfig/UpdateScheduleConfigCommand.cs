using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Doctors.Command.UpdateScheduleConfig
{
    [Authorize(Roles = [Roles.Doctor])]
    public record UpdateScheduleConfigCommand(
        Guid DoctorId,
        TimeSpan DayStart,
        TimeSpan DayEnd,
        TimeSpan LunchStart,
        TimeSpan LunchEnd,
        int[] WorkingDays,
        int SlotDurationMinutes,
        int BufferMinutes
    ) : IRequest<Unit>;
}
