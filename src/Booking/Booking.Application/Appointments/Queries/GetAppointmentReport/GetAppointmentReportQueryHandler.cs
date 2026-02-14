using Booking.Application.Appointments.Common.Interfaces;
using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetAppointmentReport
{
    public class GetAppointmentReportQueryHandler(
        IBookingDbContext dbContext,
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        IPdfGenerator pdfGenerator)
        : IRequestHandler<GetAppointmentReportQuery, ExportFileDto>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly IIdentityService _identityService = identityService;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly IPdfGenerator _pdfGenerator = pdfGenerator;

        public async Task<ExportFileDto> Handle(GetAppointmentReportQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.ApplicationUser) 
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)      
                .Include(a => a.Patient)
                    .ThenInclude(p => p.ApplicationUser) 
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken)
                ?? throw new NotFoundException("Appointment", request.AppointmentId);

            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId)) throw new UnauthorizedAccessException();

            var isAdmin = await _identityService.IsInRoleAsync(currentUserId, Roles.Admin);

            if (!isAdmin)
            {
                bool isMyDoctor = appointment.Doctor.ApplicationUserId == currentUserId;
                bool isMyPatient = appointment.Patient.ApplicationUserId == currentUserId; 

                if (!isMyDoctor && !isMyPatient)
                {
                    throw new UnauthorizedAccessException("Access denied: You are not related to this appointment.");
                }
            }

            var docUser = appointment.Doctor.ApplicationUser;
            var patUser = appointment.Patient.ApplicationUser;

            var doctorName = docUser != null ? $"{docUser.FirstName} {docUser.LastName}" : "Unknown Doctor";
            var patientName = patUser != null ? $"{patUser.FirstName} {patUser.LastName}" : "Unknown Patient";
            var specialty = appointment.Doctor.Specialty?.Name ?? "General Practice";

            var reportData = new MedicalReportDto(
                DoctorName: doctorName,
                DoctorSpecialty: specialty,
                PatientName: patientName,
                Date: appointment.StartTime,
                Diagnosis: appointment.Diagnosis ?? "Pending Diagnosis", 
                MedicalNotes: appointment.MedicalNotes ?? string.Empty,
                TreatmentPlan: appointment.TreatmentPlan ?? "No treatment plan specified.",
                PrescribedMedications: appointment.PrescribedMedications ?? "No medications prescribed."
            );

            var fileContent = _pdfGenerator.GenerateMedicalReport(reportData);
            var fileName = $"MedicalReport_{appointment.StartTime:yyyyMMdd}_{appointment.Id}.pdf";

            return new ExportFileDto(fileName, "application/pdf", fileContent);
        }
    }
}
