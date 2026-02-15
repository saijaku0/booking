namespace Booking.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
/// Saves the provided byte array as a file in storage using the given string qualifiers to determine its location.
/// </summary>
/// <param name="reportBytes">The file contents as a byte array.</param>
/// <param name="v1">First qualifier for storage placement (e.g., container, folder, or category).</param>
/// <param name="v2">Second qualifier for storage placement (e.g., subfolder, file name, or identifier).</param>
/// <returns>The stored file's URL or identifier.</returns>
Task<string> SaveFileAsync(byte[] reportBytes, string v1, string v2);
        /// <summary>
/// Deletes the file identified by the provided file URL or storage identifier.
/// </summary>
/// <param name="fileUrl">The URL or storage identifier of the file to delete.</param>
Task DeleteFile(string fileUrl);
        /// <summary>
/// Uploads a file from the provided stream to storage and returns its location.
/// </summary>
/// <param name="fileStream">Stream containing the file data to upload.</param>
/// <param name="fileName">Desired filename or path for the uploaded file.</param>
/// <param name="contentType">MIME type of the file (for example, "image/png").</param>
/// <returns>URL or identifier of the uploaded file.</returns>
Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    }
}