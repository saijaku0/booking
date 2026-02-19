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
            var userId = _userService.UserId
                ?? throw new UnauthorizedAccessException("User is not authorized.");

            var appointment = await _dbContext.Appointments
                .Include(a => a.Attachments)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken) 
                ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

            bool isPatient = appointment.Patient?.ApplicationUserId == userId;
            bool isDoctor = appointment.Doctor?.ApplicationUserId == userId;
            if (!isPatient && !isDoctor)
                throw new ForbiddenAccessException("You are not allowed to upload attachments to this appointment.");

            if (request.File == null || request.File.Length == 0)
                throw new ArgumentException("File is empty.");

            const long maxFileSize = 10 * 1024 * 1024; 
            if (request.File.Length > maxFileSize)
                throw new ArgumentException($"File size exceeds the limit of {maxFileSize / 1024 / 1024} MB.");

            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
            if (!allowedMimeTypes.Contains(request.File.ContentType))
                throw new ArgumentException("File type is not allowed.");

            var extension = Path.GetExtension(request.File.FileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                throw new ArgumentException("File extension is not allowed.");

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            await using var stream = request.File.OpenReadStream();
            var fileUrl = await _fileStorageService
                .UploadFileAsync(
                stream, 
                uniqueFileName, 
                request.File.ContentType);

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