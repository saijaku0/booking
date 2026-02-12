using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;

namespace Booking.Application.Doctors.Command.UpdateScheduleConfig
{
    public class UpdateScheduleConfigCommandHandler(
        IBookingDbContext context) : IRequestHandler<UpdateScheduleConfigCommand, Unit>
    {
        private readonly IBookingDbContext _context = context;
        public async Task<Unit> Handle(UpdateScheduleConfigCommand request, CancellationToken cancellationToken)
        {
            var config = _context.DoctorScheduleConfigs
                .FirstOrDefault(x => x.DoctorId == request.DoctorId);

            if (config == null)
            {
                config = new DoctorScheduleConfig(request.DoctorId);
                _context.DoctorScheduleConfigs.Add(config);
            }

            config.SetWorkingHours(
                request.DayStart,
                request.DayEnd,
                request.LunchStart,
                request.LunchEnd);

            config.SetWorkingDays(request.WorkingDays);

            config.SetSlotSettings(request.SlotDurationMinutes, request.BufferMinutes);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
