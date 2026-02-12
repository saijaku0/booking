using Microsoft.EntityFrameworkCore;
using Booking.Application.Common.Interfaces;
using Booking.Application.Doctors.Dtos;
using MediatR;
using Booking.Application.Common.Extension;
using Booking.Domain.Entities;

namespace Booking.Application.Doctors.Queries.GetDoctorStats
{
    public class GetDoctorStatsQueryHandler(
        IBookingDbContext dbContext)
        : IRequestHandler<GetDoctorStatsQuery, DoctorStatsDto>
    {
        private readonly IBookingDbContext _dbContext = dbContext;

        public async Task<DoctorStatsDto> Handle(
            GetDoctorStatsQuery request, 
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = DateTime.UtcNow;

            switch (request.Period.ToLower())
            {
                case "day":
                    startDate = DateTime.SpecifyKind(now.Date, DateTimeKind.Utc);
                    endDate = startDate.AddDays(1).AddTicks(-1);
                    break;
                case "week":
                    var diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                    var startOfWeek = now.AddDays(-1 * diff).Date;
                    startDate = DateTime.SpecifyKind(startOfWeek, DateTimeKind.Utc);
                    endDate = startDate.AddDays(7).AddTicks(-1);
                    break;
                case "month":
                default:
                    startDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                    break;
            }

            var doctorFee = await _dbContext.Doctors
                .Where(d => d.Id == request.DoctorId)
                .Select(d => d.ConsultationFee)
                .FirstOrDefaultAsync(cancellationToken);

            var appointments = await _dbContext.Appointments
                .AsNoTracking()
                .WhereOverlaps(request.DoctorId,
                    startDate,
                    endDate)
                .ToListAsync(cancellationToken);

            var totalPatients = appointments.Count;

            var completedCount = appointments.Count(a => a.Status == AppointmentStatus.Completed);

            return new DoctorStatsDto
            {
                Period = request.Period,
                TotalPatients = totalPatients,
                CompletedAppointments = completedCount,
                TotalEarnings = completedCount * doctorFee
            };
        }
    }
}
