using Booking.Application.Doctors.Dtos;
using MediatR;

namespace Booking.Application.Doctors.Queries.GetDoctorById
{
    public record GetDoctorByIdQuery(Guid Id)
        : IRequest<DoctorDto>;
}
