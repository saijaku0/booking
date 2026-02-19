using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Appointments.Commands.DeleteAttachment
{
    [Authorize(Roles = [Roles.Doctor])]
    public record DeleteAttachmentCommand(
        Guid AppointmentId,
        Guid AttachmentId
    ) : IRequest;
}
