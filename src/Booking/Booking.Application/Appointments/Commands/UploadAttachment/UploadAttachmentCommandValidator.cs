using FluentValidation;

namespace Booking.Application.Appointments.Commands.UploadAttachment
{
    public class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
    {
        private const int MaxFileSize = 10 * 1024 * 1024; // 10 MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        /// <summary>
        /// Configures validation rules for UploadAttachmentCommand.
        /// </summary>
        /// <remarks>
        /// Ensures AppointmentId is not empty; File is provided and not null; File length is greater than 0 and does not exceed 10 MB; and the file extension is one of: .jpg, .jpeg, .png, .pdf.
        /// </remarks>
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