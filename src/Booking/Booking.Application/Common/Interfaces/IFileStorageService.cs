namespace Booking.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        public Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    }
}
