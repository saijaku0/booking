namespace Booking.Domain.Entities
{
    public class AppointmentAttachment
    {
        public Guid Id { get; init; }
        public Guid AppointmentId { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public string FileType { get; private set; } = string.Empty;

        private static readonly HashSet<string> _allowedFileTypes = [ 
            "image/jpeg", 
            "image/png", 
            "application/pdf" 
        ];

        private AppointmentAttachment() { }

        public AppointmentAttachment(
            Guid appointmentId,
            string fileName,
            string filePath,
            string fileType)
        {
            Id = Guid.NewGuid();
            AppointmentId = appointmentId;

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

        public void SetFileType(string fileType)
        {
            if (string.IsNullOrWhiteSpace(fileType))
                throw new ArgumentException("File type cannot be empty.");

            var normalizedFileType = fileType.ToLowerInvariant();
            if(!_allowedFileTypes.Contains(normalizedFileType))
                throw new ArgumentException($"File type '{fileType}' is not allowed. Allowed types are: {string.Join(", ", _allowedFileTypes)}.");

            FileType = normalizedFileType;
        }
    }
}
