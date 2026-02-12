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
            var query = from a in _dbContext.Appointments
                        join d in _dbContext.Doctors on a.DoctorId equals d.Id 
                        join s in _dbContext.Specialties on d.SpecialtyId equals s.SpecialtyId
                        where a.Id == request.AppointmentId
                        select new
                        {
                            Appointment = a,
                            DoctorName = $"{d.ApplicationUser.FirstName} {d.ApplicationUser.LastName}",
                            DoctorUserId = d.ApplicationUserId, 
                            SpecialtyName = s.Name,
                            PatientId = a.CustomerId
                        };

            var data = await query.FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Appointment", request.AppointmentId);

            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId)) throw new UnauthorizedAccessException();

            var isAdmin = await _identityService.IsInRoleAsync(currentUserId, Roles.Admin);
            var isDoctor = await _identityService.IsInRoleAsync(currentUserId, Roles.Doctor);

            if (!isAdmin)
            {
                if (isDoctor)
                {
                    if (data.DoctorUserId != currentUserId)
                        throw new UnauthorizedAccessException("Doctors can only access reports for their own appointments.");
                }
                else
                {
                    if (data.PatientId.ToString() != currentUserId)
                        throw new UnauthorizedAccessException("You can only access your own medical reports.");
                }
            }

            var patientName = await _identityService.GetUserNameAsync(data.PatientId.ToString()) ?? "Unknown";

            var reportData = new MedicalReportDto(
                DoctorName: data.DoctorName,      
                DoctorSpecialty: data.SpecialtyName, 
                PatientName: patientName,
                Date: data.Appointment.StartTime,
                Diagnosis: data.Appointment.MedicalNotes ?? "No notes.",
                MedicalNotes: data.Appointment.MedicalNotes ?? string.Empty,
                TreatmentPlan: "Standard recovery procedures."
            );

            var fileContent = _pdfGenerator.GenerateMedicalReport(reportData);
            var fileName = $"MedicalReport_{data.Appointment.StartTime:yyyyMMdd}_{data.Appointment.Id}.pdf";

            return new ExportFileDto(fileName, "application/pdf", fileContent);
        }
    }
}
