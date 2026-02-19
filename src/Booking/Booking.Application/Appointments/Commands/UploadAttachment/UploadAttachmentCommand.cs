using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Booking.Application.Appointments.Commands.UploadAttachment
{
    [Authorize(Roles = [Roles.Doctor])]
    public record UploadAttachmentCommand(
        IFormFile File, 
        Guid AppointmentId,
        AttachmentType FileType
     ) : IRequest<Guid>;
}
