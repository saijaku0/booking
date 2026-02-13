using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Extension;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetAppointmentsByDate
{
    public class GetAppointmentsByDateQueryHandler(
        IBookingDbContext context,
        IIdentityService identityService) 
        : IRequestHandler<GetAppointmentsByDateQuery, List<AppointmentDto>>
    {
        private readonly IBookingDbContext _context = context;
        private readonly IIdentityService _identityService = identityService;

        public async Task<List<AppointmentDto>> Handle(GetAppointmentsByDateQuery request, CancellationToken cancellationToken)
        {
            var doctorInfo = await _context.Doctors
            .Include(d => d.Specialty) 
            .Where(d => d.Id == request.DoctorId)
            .Select(d => new
            {
                FullName = $"{d.ApplicationUser.FirstName} {d.ApplicationUser.LastName}",
                Specialty = d.Specialty != null ? d.Specialty.Name : "General"
            })
            .FirstOrDefaultAsync(cancellationToken);

            string docName = doctorInfo?.FullName ?? "Unknown Doctor";
            string docSpecialty = doctorInfo?.Specialty ?? "Unknown";

            var appointments = await _context.Appointments
                .AsNoTracking()
                .Where(a => a.Status != AppointmentStatus.Canceled)
                .Where(a => a.DoctorId == request.DoctorId)
                .WhereOverlaps(request.DoctorId,
                    request.StartTime,
                    request.EndTime) 
                .ToListAsync(cancellationToken);

            var result = new List<AppointmentDto>();

            foreach (var app in appointments)
            {
                var patientName = await _identityService.GetUserNameAsync(app.PatientId.ToString())
                                  ?? "Unknown Patient";

                result.Add(new AppointmentDto
                {
                    Id = app.Id,
                    DoctorId = app.DoctorId,
                    PatientId = app.PatientId,

                    DoctorName = docName,
                    Specialty = docSpecialty,
                    PatientName = patientName, 

                    Status = app.Status.ToString(),
                    MedicalNotes = app.MedicalNotes ?? string.Empty,
                    StartTime = app.StartTime,
                    EndTime = app.EndTime
                });
            }

            return result.OrderBy(a => a.StartTime).ToList();
        }
    }
}
