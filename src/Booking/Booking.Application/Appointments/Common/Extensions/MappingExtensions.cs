using Booking.Application.Appointments.Dtos;
using Booking.Domain.Entities;

namespace Booking.Application.Appointments.Common.Extensions
{
    public static class MappingExtensions
    {
        public static List<AttachmentDto> ToAttachmentDtos(this IEnumerable<AppointmentAttachment> attachments)
        {
            if (attachments == null)
                return [];

            return [.. attachments.Select(a => new AttachmentDto
        {
            Id = a.Id,
            FileName = a.FileName,
            FileType = a.FileType,
            CreatedAt = a.DateCreated
        })];
        }
    }
}
