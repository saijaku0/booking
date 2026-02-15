using MediatR;

namespace Booking.Application.Doctors.Command.DeleteDoctor
{
    public record DeleteDoctorCommand(Guid DoctorId) : IRequest; 
}
