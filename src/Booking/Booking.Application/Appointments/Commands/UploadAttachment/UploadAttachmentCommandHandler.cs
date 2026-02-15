using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Commands.UploadAttachment
{
    public class UploadAttachmentCommandHandler(
        IBookingDbContext dbContext,
        IFileStorageService fileStorageService,
        ICurrentUserService userService) : IRequestHandler<UploadAttachmentCommand, Guid>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly ICurrentUserService _userService = userService;

        public async Task<Guid> Handle(
            UploadAttachmentCommand request, 
            CancellationToken cancellationToken)
        {
            var appointment = await _dbContext.Appointments
                .Include(a => a.Attachments)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken) 
                ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

            var userId = _userService.UserId 
                ?? throw new UnauthorizedAccessException("User is not authorized");

            bool isPatient = appointment.Patient.ApplicationUserId == userId;
            bool isDoctorAttachment = appointment.Doctor.ApplicationUserId == userId;

            if (!isPatient && !isDoctorAttachment)
                throw new UnauthorizedAccessException("You can only upload attachments to your own appointments.");
            
            var extension = Path.GetExtension(request.File.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            using var stream = request.File.OpenReadStream();
            var fileUrl = await _fileStorageService
                .UploadFileAsync(stream, uniqueFileName, request.File.ContentType);

            var attachment = appointment.AddAttachment
            (
                fileName: request.File.FileName,
                filePath: fileUrl,
                fileType: request.File.ContentType,
                type: request.FileType
            );

            await _dbContext.SaveChangesAsync(cancellationToken);
            return attachment.Id;
        }
    }
}
