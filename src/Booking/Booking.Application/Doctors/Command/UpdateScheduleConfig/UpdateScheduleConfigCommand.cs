using MediatR;

namespace Booking.Application.Doctors.Command.UpdateScheduleConfig
{
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
