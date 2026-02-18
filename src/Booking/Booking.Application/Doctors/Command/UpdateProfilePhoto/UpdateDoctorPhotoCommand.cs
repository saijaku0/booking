using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Doctors.Command.UpdateProfilePhoto
{
    [Authorize(Roles = [Roles.Admin, Roles.Doctor])]
    public record UpdateDoctorPhotoCommand(
        Stream PhotoStream,
        string FileName,
        string ContentType) : IRequest<Unit>;
}
