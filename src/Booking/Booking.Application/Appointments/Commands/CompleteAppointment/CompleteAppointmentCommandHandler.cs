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
                .Include(a => a.Attachments)
                .Include(a => a.Patient).ThenInclude(p => p.ApplicationUser)
                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                .Include(a => a.Doctor).ThenInclude(d => d.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken)
                ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

            var userId = _userService.UserId 
                ?? throw new UnauthorizedAccessException("User is not authorized");

            if (appointment.Doctor == null)
                throw new InvalidOperationException("Appointment has no associated doctor.");

            var isAdmin = await _identityService.IsInRoleAsync(userId, Roles.Admin);
            var isOwner = appointment.Doctor.ApplicationUserId == userId;
            if (!isAdmin && !isOwner)
                throw new UnauthorizedAccessException("You can only complete your own appointments.");

            appointment.Complete(
                request.Diagnosis, 
                request.MedicalNotes, 
                request.TreatmentPlan, 
                request.PrescribedMedications);

            var reportDto = new MedicalReportDto(
                DoctorName: appointment.Doctor.ApplicationUser.FullName,
                DoctorSpecialty: appointment.Doctor.Specialty.Name,
                PatientName: appointment.Patient.ApplicationUser.FullName,
                Date: DateTime.UtcNow,
                Diagnosis: appointment.Diagnosis!,
                MedicalNotes: appointment.MedicalNotes ?? "",
                TreatmentPlan: appointment.TreatmentPlan ?? "",
                PrescribedMedications: appointment.PrescribedMedications ?? ""
            );

            var reportBytes = _pdfService.GenerateMedicalReport(reportDto);
            var reportFileName = $"MedicalReport_{appointment.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            string fileType = "application/pdf";

            var reportPath = await _fileService.SaveFileAsync(
                reportBytes,
                reportFileName,
                fileType);

            var reportAttachment = appointment.AddAttachment(
                fileName: reportFileName,
                filePath: reportPath,
                fileType: fileType,
                type: AttachmentType.MedicalReport
            );

            _dbContext.AppointmentAttachments.Add(reportAttachment);

            if (!string.IsNullOrWhiteSpace(request.PrescribedMedications))
            {
                var prescriptionBytes = _pdfService.GeneratePrescription(reportDto);
                var prescriptionFileName = $"Prescription_{appointment.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

                var prescriptionPath = await _fileService.SaveFileAsync(
                    prescriptionBytes,
                    prescriptionFileName,
                    fileType);

                var prescriptionAttachment = appointment.AddAttachment(
                    fileName: prescriptionFileName,
                    filePath: prescriptionPath,
                    fileType: fileType,
                    type: AttachmentType.Prescription
                );

                _dbContext.AppointmentAttachments.Add(prescriptionAttachment);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
