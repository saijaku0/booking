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
                    var diff = now.DayOfWeek == DayOfWeek.Sunday ? -6 : (int)DayOfWeek.Monday - (int)now.DayOfWeek;
                    startDate = DateTime.SpecifyKind(now.Date.AddDays(diff), DateTimeKind.Utc);
                    endDate = startDate.AddDays(7).AddTicks(-1);
                    break;

                case "month":
                    startDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                    break;

                case "total":
                case "all":
                    break;

                default:
                    startDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                    break;
            }

            var doctorFee = await _dbContext.Doctors
                .Where(d => d.Id == request.DoctorId)
                .Select(d => d.ConsultationFee)
                .FirstOrDefaultAsync(cancellationToken);

            var appointmentsQuery = _dbContext.Appointments
                .AsNoTracking()
                .WhereOverlaps(request.DoctorId,
                    startDate,
                    endDate)
                .Where(a => a.Status == AppointmentStatus.Completed);

            if (request.Period?.ToLower() != "total" && request.Period?.ToLower() != "all")
            {
                appointmentsQuery = appointmentsQuery   
                    .Where(a => a.StartTime >= startDate && a.StartTime <= endDate);
            }

            var completedAppointments = await appointmentsQuery.ToListAsync(cancellationToken);
            var completedCount = completedAppointments.Count;
            var totalEarnings = completedCount * doctorFee;

            return new DoctorStatsDto
            {
                Period = request.Period,
                TotalPatients = completedCount,
                CompletedAppointments = completedCount,
                TotalEarnings = totalEarnings
            };
        }
    }
}
