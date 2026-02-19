using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Doctors.Command.UpdateProfilePhoto
{
    public class UpdateDoctorPhotoCommandHandler(
        IBookingDbContext dbContext,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService,
        IIdentityService identityService) 
        : IRequestHandler<UpdateDoctorPhotoCommand, Unit>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly IIdentityService _identityService = identityService;

        public async Task<Unit> Handle(
            UpdateDoctorPhotoCommand request, 
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException("User is not authorized.");

            var doctor = await _dbContext.Doctors
                .FirstOrDefaultAsync(d => d.ApplicationUserId == currentUserId, cancellationToken)
                ?? throw new NotFoundException(nameof(Doctor), $"User {currentUserId}");

            var isAdmin = await _identityService.IsInRoleAsync(currentUserId, Roles.Admin);
            var isOwner = doctor.ApplicationUserId == currentUserId;
            if (!isOwner && !isAdmin)
                throw new ForbiddenAccessException("You can only update your own photo.");

            if (request.PhotoStream == null || request.PhotoStream.Length == 0)
                throw new ArgumentException("File is empty.");

            var allowExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(request.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !allowExtensions.Contains(fileExtension))
                throw new ArgumentException("Only images are allowed.");

            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedMimeTypes.Contains(request.ContentType))
                throw new ArgumentException("Invalid image MIME type.");

            var oldPhotoUrl = doctor.ImageUrl;

            var photoUrl = await _fileStorageService
                .UploadFileAsync(
                    request.PhotoStream,
                    request.FileName,
                    request.ContentType);
            doctor.SetImageUrl(photoUrl);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
