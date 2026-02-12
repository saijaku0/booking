namespace Booking.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(byte[] reportBytes, string v1, string v2);
        public Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    }
}
