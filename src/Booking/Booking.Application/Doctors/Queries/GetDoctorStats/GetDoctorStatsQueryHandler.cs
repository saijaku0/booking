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
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = DateTime.UtcNow;

            switch (request.Period.ToLower())
            {
                case "day":
                    startDate = DateTime.UtcNow.Date;
                    endDate = startDate.AddDays(1);
                    break;
                case "week":
                    int diff = (7 + (DateTime.UtcNow.DayOfWeek - DayOfWeek.Monday)) % 7;
                    startDate = DateTime.UtcNow.AddDays(-1 * diff).Date;
                    endDate = startDate.AddDays(7);
                    break;
                case "month":
                    startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    endDate = startDate.AddMonths(1);
                    break;
                default:
                    throw new ArgumentException("Invalid period specified. Use 'day', 'week', or 'month'.");
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
