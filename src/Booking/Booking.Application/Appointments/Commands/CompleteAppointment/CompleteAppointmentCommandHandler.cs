using Booking.Application.Appointments.Common.Interfaces;
using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Commands.CompleteAppointment
{
    public class CompleteAppointmentCommandHandler(
        IBookingDbContext dbContext,
        ICurrentUserService userService,
        IIdentityService identityService,
        IPdfGenerator pdfService,
        IFileStorageService fileService)
        : IRequestHandler<CompleteAppointmentCommand>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly ICurrentUserService _userService = userService;
        private readonly IIdentityService _identityService = identityService;
        private readonly IPdfGenerator _pdfService = pdfService;
        private readonly IFileStorageService _fileService = fileService;

        public async Task Handle(
            CompleteAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.ApplicationUser)
                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                .Include(a => a.Doctor).ThenInclude(d => d.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken)
                ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

            var userId = _userService.UserId ?? throw new UnauthorizedAccessException("User is not authorized");
            var isAdmin = await _identityService.IsInRoleAsync(userId, Roles.Admin);

            if (!isAdmin)
            {
                if (appointment.Doctor.ApplicationUser?.Id != userId)
                    throw new UnauthorizedAccessException("You can only complete your own appointments.");
            }

            appointment.Complete(request.Diagnosis, request.MedicalNotes, request.TreatmentPlan);

            if (appointment == null)
                throw new NotFoundException(nameof(Appointment), request.AppointmentId);

            var reportDto = new MedicalReportDto(
                DoctorName: $"{appointment.Doctor.ApplicationUser.FirstName} {appointment.Doctor.ApplicationUser.LastName}",
                DoctorSpecialty: appointment.Doctor.Specialty.Name,
                PatientName: $"{appointment.Patient.ApplicationUser.FirstName} {appointment.Patient.ApplicationUser.LastName}",
                Date: DateTime.Now,

                Diagnosis: appointment.Diagnosis!,
                MedicalNotes: appointment.MedicalNotes,
                TreatmentPlan: appointment.TreatmentPlan
            );

            var reportBytes = _pdfService.GenerateMedicalReport(reportDto);

            var reportPath = await _fileService.SaveFileAsync(
                reportBytes,
                $"Report_{appointment.Id}.pdf",
                "application/pdf");

            appointment.AddAttachment(
                fileName: "Medical_Report.pdf",
                filePath: reportPath,
                fileType: "application/pdf",
                type: AttachmentType.MedicalReport
            );

            if (!string.IsNullOrWhiteSpace(request.TreatmentPlan))
            {
                var prescriptionBytes = _pdfService.GeneratePrescription(reportDto);

                var prescriptionPath = await _fileService.SaveFileAsync(
                    prescriptionBytes,
                    $"Prescription_{appointment.Id}.pdf",
                    "application/pdf");

                appointment.AddAttachment(
                    fileName: "Prescription.pdf",
                    filePath: prescriptionPath,
                    fileType: "application/pdf",
                    type: AttachmentType.Prescription
                );
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
