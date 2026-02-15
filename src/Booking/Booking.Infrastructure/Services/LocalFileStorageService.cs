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