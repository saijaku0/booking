using Booking.Application.Common.Interfaces;
using Booking.Application.Doctors.Dtos;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Doctors.Queries.GetDoctorSlots
{
    internal class GetDoctorSlotsQueryHandler (
        IBookingDbContext dbContext) : IRequestHandler<GetDoctorSlotsQuery, List<DoctorTimeSlotDto>>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        public async Task<List<DoctorTimeSlotDto>> Handle(
            GetDoctorSlotsQuery request, 
            CancellationToken cancellationToken)
        {
            var config = _dbContext.DoctorScheduleConfigs
                .AsNoTracking()
                .FirstOrDefault(x => x.DoctorId == request.DoctorId)
                ?? throw new KeyNotFoundException("Doctor schedule config not found.");

            if(!config.IsWorkingDay(DateOnly.FromDateTime(request.Date)))
                return new List<DoctorTimeSlotDto>();

            var baseDate = request.Date.Date;
            var workStart = baseDate.Add(config.DayStart);
            var workEnd = baseDate.Add(config.DayEnd);
            var lunchStart = baseDate.Add(config.LunchStart);
            var lunchEnd = baseDate.Add(config.LunchEnd);

            var slotDuration = TimeSpan.FromMinutes(config.SlotDurationMinutes);
            var buffer = TimeSpan.FromMinutes(config.BufferMinutes);
            var now = DateTime.UtcNow;

            var existingAppointments = await _dbContext.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorId == request.DoctorId)
                .Where(a => a.Status != AppointmentStatus.Canceled)
                .Where(a => a.StartTime < workEnd && a.EndTime > workStart)
                .ToListAsync(cancellationToken);

            var availableSlots = new List<DoctorTimeSlotDto>();
            var currentSlotStart = workStart;

            while (currentSlotStart.Add(slotDuration) <= workEnd)
            {
                var currentSlotEnd = currentSlotStart.Add(slotDuration);
                bool overlapsLunch = currentSlotStart < lunchEnd && currentSlotEnd > lunchStart;

                if (overlapsLunch)
                {
                    if (currentSlotStart < lunchEnd)
                        currentSlotStart = lunchEnd;
                    else
                        currentSlotStart = currentSlotStart.Add(slotDuration);
                    continue; 
                }

                bool isTooLateToBook = currentSlotStart < now.AddHours(config.MinHoursInAdvance);
                bool isTooFarInFuture = currentSlotStart > now.AddDays(config.MaxDaysInAdvance);
                bool isTaken = existingAppointments.Any(app =>
                    app.StartTime < currentSlotEnd && app.EndTime > currentSlotStart);

                if (!isTaken && !isTooLateToBook && !isTooFarInFuture)
                    availableSlots.Add(new DoctorTimeSlotDto
                    {
                        Start = currentSlotStart,
                        End = currentSlotEnd,
                        IsAvailable = true
                    });

                currentSlotStart = currentSlotEnd.Add(buffer);
            }

            return availableSlots;
        }
    }
}
