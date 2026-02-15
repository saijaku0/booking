
namespace Booking.Application.Appointments.Dtos
{
    public record AppointmentDetailDto : AppointmentBaseDto
    {
        public List<AttachmentDto> Attachments { get; init; } = [];

    }
}
