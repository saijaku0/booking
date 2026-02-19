using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Appointments.Commands.DeleteAttachment
{
    public class DeleteAttachmentCommandHandler(
        IBookingDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        ILogger<DeleteAttachmentCommandHandler> logger
    ) : IRequestHandler<DeleteAttachmentCommand>
    {
        private readonly IBookingDbContext _context = context;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly ILogger<DeleteAttachmentCommandHandler> _logger = logger;

        public async Task Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var appointment = await _context.Appointments
                .Include(a => a.Attachments)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken)
                ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

            bool isDoctor = appointment.Doctor.ApplicationUserId == userId;
            if (!isDoctor)
                throw new ForbiddenAccessException("Only the doctor can delete attachments.");

            var attachment = appointment.Attachments
                .FirstOrDefault(a => a.Id == request.AttachmentId)
                ?? throw new NotFoundException(nameof(AppointmentAttachment), request.AttachmentId);

            var filePath = attachment.FilePath;

            appointment.RemoveAttachment(attachment.Id);
            await _context.SaveChangesAsync(cancellationToken);

            try
            {
                await _fileStorageService.DeleteFile(filePath);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to delete file from disk");
            }

        }
    }
}