namespace Booking.Domain.Entities
{
    public class AppointmentAttachment
    {
        public Guid Id { get; init; }
        public Guid AppointmentId { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public string FileType { get; private set; } = string.Empty;
        public DateTime DateCreated { get; private set; }
        public AttachmentType Type { get; private set; }

        private static readonly HashSet<string> _allowedFileTypes = [ 
            "image/jpeg", 
            "image/png", 
            "application/pdf" 
        ];

        private AppointmentAttachment() { }

        /// <summary>
        /// Initializes a new AppointmentAttachment for the specified appointment and file, generating a new Id and recording creation time.
        /// </summary>
        /// <param name="appointmentId">Identifier of the appointment this attachment belongs to.</param>
        /// <param name="fileName">The file's name; cannot be null, empty, or whitespace.</param>
        /// <param name="filePath">The file's storage path; cannot be null, empty, or whitespace.</param>
        /// <param name="fileType">The file's MIME type; must be one of "image/jpeg", "image/png", or "application/pdf".</param>
        /// <param name="type">The attachment's domain-specific type.</param>
        /// <exception cref="ArgumentException">Thrown if fileName, filePath, or fileType is null, empty, whitespace, or if fileType is not an allowed type.</exception>
        public AppointmentAttachment(
            Guid appointmentId,
            string fileName,
            string filePath,
            string fileType,
            AttachmentType type)
        {
            Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
            AppointmentId = appointmentId;

            Type = type;

            SetFileName(fileName);
            SetFilePath(filePath);
            SetFileType(fileType);
        }

        public void SetFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty.");
            FilePath = filePath;
        }

        public void SetFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.");
            FileName = fileName;
        }

        /// <summary>
        /// Validates the provided MIME file type and assigns it to the attachment.
        /// </summary>
        /// <param name="fileType">The MIME type to set (e.g., "image/png", "application/pdf").</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="fileType"/> is empty or whitespace, or when it is not one of the allowed MIME types.
        /// </exception>
        public void SetFileType(string fileType)
        {
            if (string.IsNullOrWhiteSpace(fileType))
                throw new ArgumentException("File type cannot be empty.");

            var normalizedFileType = fileType.ToLowerInvariant();
            if(!_allowedFileTypes.Contains(normalizedFileType))
                throw new ArgumentException($"File type '{fileType}' is not allowed. Allowed types are: {string.Join(", ", _allowedFileTypes)}.");

            FileType = normalizedFileType;
        }

        /// <summary>
        /// Removes attachment metadata for the specified file name after confirming the name matches and the physical file is not present on disk.
        /// </summary>
        /// <param name="fileName">The name of the file to remove from the attachment record.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="fileName"/> does not match the attachment's current <see cref="FileName"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a file with the specified name still exists on the file system; the file must be deleted from storage before removing the attachment.</exception>
        public void DeleteFile(string fileName)
        {
            if(fileName != FileName)
                throw new ArgumentException("The specified file name does not match the attachment's file name.");
            if (File.Exists(fileName))
                throw new InvalidOperationException("Cannot delete file because it still exists on the file system. Please ensure the file is deleted from storage before removing the attachment.");

            FilePath = string.Empty;
            FileName = string.Empty;
            FileType = string.Empty;
        }
    }
}