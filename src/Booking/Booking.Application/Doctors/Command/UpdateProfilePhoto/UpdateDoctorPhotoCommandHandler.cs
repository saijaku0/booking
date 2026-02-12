using Booking.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Doctors.Command.UpdateProfilePhoto
{
    public class UpdateDoctorPhotoCommandHandler(
        IBookingDbContext dbContext,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService) 
        : IRequestHandler<UpdateDoctorPhotoCommand, Unit>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<Unit> Handle(
            UpdateDoctorPhotoCommand request, 
            CancellationToken cancellationToken)
        {
            var doctorId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(doctorId))
                throw new UnauthorizedAccessException("User is not authorized.");

            var doctor = await _dbContext.Doctors
                .FirstOrDefaultAsync(d => d.ApplicationUserId == doctorId, cancellationToken)
                ?? throw new KeyNotFoundException("Doctor not found.");

            var allowExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(request.FileName)?.ToLowerInvariant();

            if (string.IsNullOrEmpty(fileExtension) || !allowExtensions.Contains(fileExtension))
                throw new ArgumentException("Only images are allowed.");

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
