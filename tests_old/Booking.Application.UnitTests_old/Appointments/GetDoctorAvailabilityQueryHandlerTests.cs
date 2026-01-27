using Booking.Application.Appointments.Queries.GetDoctorAvailability;
using Booking.Domain.Entities;
using Booking.Domain.Enums; 
using Booking.Infrastructure.Data; 
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Booking.Application.UnitTests.Appointments;

public class GetDoctorAvailabilityQueryHandlerTests
{
    private readonly ApplicationDbContext _context;
    private readonly GetDoctorAvailabilityQueryHandler _handler;

    public GetDoctorAvailabilityQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        _handler = new GetDoctorAvailabilityQueryHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldReturnSlots_WhenAppointmentIsCanceled()
    {
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var date = DateTime.Today.AddDays(1);

        // Сценарий:
        // 1. Запись на 10:00 (Активная) -> Слот должен пропасть
        // 2. Запись на 12:00 (Отмененная) -> Слот должен остаться!

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

        // 1. Слот 10:00 должен отсутствовать (он занят активной записью)
        result.Should().NotContain(slot => slot.Start.Hour == 10 && slot.Start.Minute == 0);

        // 2. Слот 12:00 должен присутствовать (запись была отменена!)
        result.Should().Contain(slot => slot.Start.Hour == 12 && slot.Start.Minute == 0);

        // 3. Проверка количества
        // Всего слотов с 9 до 17 (8 часов * 2) = 16 слотов.
        // Занят только 1 (на 10:00).
        // Должно вернуться 15.
        result.Count.Should().Be(15);
    }
}