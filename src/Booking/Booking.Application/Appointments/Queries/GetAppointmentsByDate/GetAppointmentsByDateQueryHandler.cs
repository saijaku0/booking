using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Extension;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetAppointmentsByDate
{
    public class GetAppointmentsByDateQueryHandler(
        IBookingDbContext context) 
        : IRequestHandler<GetAppointmentsByDateQuery, List<AppointmentListDto>>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<List<AppointmentListDto>> Handle(
            GetAppointmentsByDateQuery request, 
            CancellationToken cancellationToken)
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
                .Include(a => a.Patient)
                    .ThenInclude(p => p.ApplicationUser)
                .Where(a => a.Status != AppointmentStatus.Canceled)
                .Where(a => a.DoctorId == request.DoctorId)
                .WhereOverlaps(request.DoctorId,
                    request.StartTime,
                    request.EndTime) 
                .ToListAsync(cancellationToken);

            var result = new List<AppointmentListDto>();

            foreach (var app in appointments)
            {
                var patientName = app.Patient?.ApplicationUser != null
                    ? $"{app.Patient.ApplicationUser.FirstName} {app.Patient.ApplicationUser.LastName}"
                    : "Unknown Patient";

                result.Add(new AppointmentListDto
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
