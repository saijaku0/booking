using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetDoctorAvailability
{
    public class GetDoctorAvailabilityQueryHandler(
        IBookingDbContext context)
        : IRequestHandler<GetDoctorAvailabilityQuery, List<TimeSlotDto>>
    {
        private readonly IBookingDbContext _context = context;

        private readonly TimeSpan _slotDuration = TimeSpan.FromMinutes(30);
        private readonly int _startHour = 9; // 9 AM
        private readonly int _endHour = 17; // 5 PM

        public async Task<List<TimeSlotDto>> Handle(
            GetDoctorAvailabilityQuery request,
            CancellationToken cancellationToken)
        {
            var workDayStart = request.Date.Date.AddHours(_startHour);
            var workDayEnd = request.Date.Date.AddHours(_endHour);

            var bookedSlots = await _context.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorId == request.DoctorId)
                .Where(a => a.Status != AppointmentStatus.Canceled) 
                .Where(a => a.StartTime < workDayEnd && a.EndTime > workDayStart) 
                .ToListAsync(cancellationToken);

            var availableSlots = new List<TimeSlotDto>();
            var currentSlotStart = workDayStart;

            while (currentSlotStart.Add(_slotDuration) <= workDayEnd)
            {
                var currentSlotEnd = currentSlotStart.Add(_slotDuration);

                bool isTaken = bookedSlots.Any(booked =>
                booked.StartTime < currentSlotEnd &&
                booked.EndTime > currentSlotStart);
                if (!isTaken)
                {
                    availableSlots.Add(new TimeSlotDto
                    {
                        Start = currentSlotStart,
                        End = currentSlotEnd
                    });
                }

                currentSlotStart = currentSlotStart.Add(_slotDuration);
            }

            return availableSlots;
        }
    }
}
