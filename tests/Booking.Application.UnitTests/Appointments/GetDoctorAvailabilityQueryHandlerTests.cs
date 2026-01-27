using Booking.Application.Appointments.Queries.GetDoctorAvailability;
using Booking.Domain.Entities;
using Booking.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Booking.Application.UnitTests.Appointments;

public class GetDoctorAvailabilityQueryHandlerTests
{
    private readonly BookingDbContext _context;
    private readonly GetDoctorAvailabilityQueryHandler _handler;

    public GetDoctorAvailabilityQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BookingDbContext(options);

        _handler = new GetDoctorAvailabilityQueryHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldReturnSlots_WhenAppointmentIsCanceled()
    {
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var date = DateTime.Today.AddDays(1);

        // Script:
        // 1. Appointment at 10:00 (Active) -> Slot should disappear
        // 2. Appointment at 12:00 (Canceled) -> Slot should remain!

        var activeAppointment = new Appointment(
            doctorId,
            patientId,
            date.AddHours(10),            // 10:00
            date.AddHours(10).AddMinutes(30) // 10:30
        );

        var canceledAppointment = new Appointment(
            doctorId,
            patientId,
            date.AddHours(12),            // 12:00
            date.AddHours(12).AddMinutes(30) // 12:30
        );
        canceledAppointment.Cancel();

        await _context.Appointments.AddRangeAsync(activeAppointment, canceledAppointment);
        await _context.SaveChangesAsync();

        var query = new GetDoctorAvailabilityQuery(doctorId, date);

        var result = await _handler.Handle(query, CancellationToken.None);

        // 1. Slot 10:00 must be absent (it is occupied by an active appointment)
        result.Should().NotContain(slot => slot.Start.Hour == 10 && slot.Start.Minute == 0);

        // 2. Slot 12:00 must be present (the appointment was canceled!)
        result.Should().Contain(slot => slot.Start.Hour == 12 && slot.Start.Minute == 0);

        // 3. Check the count
        // Total slots from 9 to 17 (8 hours * 2) = 16 slots.
        // Only 1 is occupied (at 10:00).
        // Должно вернуться 15.
        result.Count.Should().Be(15);
    }
}