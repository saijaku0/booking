namespace Booking.Application.Appointments.Dtos
{
    public record ExportFileDto(
        string FileName,
        string ContentType,
        byte[] Content);
}
