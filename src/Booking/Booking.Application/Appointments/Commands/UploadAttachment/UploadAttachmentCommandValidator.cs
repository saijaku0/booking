using FluentValidation;

namespace Booking.Application.Appointments.Commands.UploadAttachment
{
    public class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
    {
        private const int MaxFileSize = 10 * 1024 * 1024; // 10 MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        public UploadAttachmentCommandValidator()
        {
            RuleFor(x => x.AppointmentId)
                .NotEmpty();

            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("File is required.");

            RuleFor(x => x.File)
                .Must(file => file.Length > 0)
                .WithMessage("File cannot be empty.")
                .Must(file => file.Length <= MaxFileSize)
                .WithMessage($"File size must not exceed {MaxFileSize / 1024 / 1024} MB.");

            RuleFor(x => x.File)
                .Must(file => _allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage($"Invalid file type. Allowed types: {string.Join(", ", _allowedExtensions)}");
        }
    }
}
