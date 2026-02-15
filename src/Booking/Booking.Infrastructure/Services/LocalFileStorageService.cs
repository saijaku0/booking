using Booking.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Booking.Infrastructure.Services
{
    public class LocalFileStorageService(IWebHostEnvironment webHostEnvironment) : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        private const string DoctorsUploadsFolder = "uploads/doctors";
        private const string DocumentsUploadsFolder = "uploads/documents";

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            return await SaveStreamAsync(fileStream, fileName, DoctorsUploadsFolder);
        }

        /// <summary>
        /// Saves the provided byte array as a uniquely named file in the documents uploads folder and returns its relative URL.
        /// </summary>
        /// <param name="content">The file content as a byte array.</param>
        /// <param name="fileName">The original file name used to determine the file extension if present.</param>
        /// <param name="contentType">The MIME type; used to infer a missing extension (for example, "application/pdf" → ".pdf").</param>
        /// <returns>The relative URL path of the stored file (for example, "/uploads/documents/{uniqueFileName}").</returns>
        public async Task<string> SaveFileAsync(byte[] content, string fileName, string contentType)
        {
            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, DocumentsUploadsFolder);

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension) && contentType == "application/pdf")
                extension = ".pdf";

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            await File.WriteAllBytesAsync(filePath, content);

            return $"/{DocumentsUploadsFolder}/{uniqueFileName}";
        }

        /// <summary>
        /// Deletes a file under the application's web root identified by the given relative URL.
        /// </summary>
        /// <param name="fileUrl">Relative URL or path to the file (for example "/uploads/documents/file.pdf"). If null or empty, no action is taken.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when the resolved file path is outside the application's web root.</exception>
        public Task DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return Task.CompletedTask;

            var fullPath = Path.GetFullPath(
                Path.Combine(_webHostEnvironment.WebRootPath, fileUrl.TrimStart('/'))
            );

            if (!fullPath.StartsWith(_webHostEnvironment.WebRootPath))
                throw new UnauthorizedAccessException("Invalid file path.");

            if (File.Exists(fullPath))
                File.Delete(fullPath);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Saves the provided stream to the specified uploads folder under the application's web root using a GUID-based filename that preserves the original file extension, and returns the file's relative URL.
        /// </summary>
        /// <param name="fileStream">Stream containing the file data to save.</param>
        /// <param name="fileName">Original filename used to determine the file extension.</param>
        /// <param name="folder">Relative folder path under the web root where the file will be stored (e.g., "uploads/documents").</param>
        /// <returns>The relative URL path to the stored file (e.g., "/{folder}/{generatedFileName}").</returns>
        private async Task<string> SaveStreamAsync(Stream fileStream, string fileName, string folder)
        {
            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, folder);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            return $"/{folder}/{uniqueFileName}";
        }
    }
}