using MediatR;

namespace Booking.Application.Doctors.Command.UpdateProfilePhoto
{
    public record UpdateDoctorPhotoCommand(
        Stream PhotoStream,
        string FileName,
        string ContentType) : IRequest<Unit>;
}
