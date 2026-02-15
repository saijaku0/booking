using MediatR;

namespace Booking.Application.Appointments.Commands.DeleteAttachment
{
    public record DeleteAttachmentCommand(
        Guid AppointmentId,
        Guid AttachmentId
    ) : IRequest;
}
