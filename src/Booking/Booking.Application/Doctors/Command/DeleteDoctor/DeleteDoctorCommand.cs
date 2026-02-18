using Booking.Application.Common.Security;
using Booking.Domain.Constants;
using MediatR;

namespace Booking.Application.Doctors.Command.DeleteDoctor
{
    [Authorize(Roles = [Roles.Admin])]
    public record DeleteDoctorCommand(Guid DoctorId) : IRequest; 
}
