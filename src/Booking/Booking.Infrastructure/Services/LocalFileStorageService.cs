using Booking.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Booking.Infrastructure.Services
{
    public class LocalFileStorageService(IWebHostEnvironment webHostEnvironment) : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private const string UploadsFolder = "uploads/doctors";

        public async Task<string> UploadFileAsync(
            Stream fileStream, 
            string fileName, 
            string contentType)
        {
            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, UploadsFolder);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);
            
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);
            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
                await fileStream.CopyToAsync(fileStreamOutput);

            return $"/{UploadsFolder}/{uniqueFileName}";
        }
    }
}
