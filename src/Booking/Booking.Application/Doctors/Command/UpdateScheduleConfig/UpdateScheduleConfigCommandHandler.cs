using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Doctors.Command.UpdateScheduleConfig
{
    public class UpdateScheduleConfigCommandHandler(
        IBookingDbContext context,
        ICurrentUserService userService) 
        : IRequestHandler<UpdateScheduleConfigCommand, Unit>
    {
        private readonly IBookingDbContext _context = context;
        private readonly ICurrentUserService _userService = userService;
        public async Task<Unit> Handle(
            UpdateScheduleConfigCommand request, 
            CancellationToken cancellationToken)
        {
            var currentUserId = _userService.UserId;
            if (string.IsNullOrWhiteSpace(currentUserId)) 
                throw new UnauthorizedAccessException();

            var doctorId = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.ApplicationUserId == currentUserId, cancellationToken)
                ?? throw new ForbiddenAccessException("Current user is not a doctor.");

            if (request.DoctorId != doctorId.Id)
                throw new ForbiddenAccessException("You can only edit your own schedule.");

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
