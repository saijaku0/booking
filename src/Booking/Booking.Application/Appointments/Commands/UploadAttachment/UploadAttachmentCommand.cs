using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Booking.Application.Appointments.Commands.UploadAttachment
{
    public record UploadAttachmentCommand(
        IFormFile File, 
        Guid AppointmentId,
        AttachmentType FileType
     ) : IRequest<Guid>;
}
