using Booking.Application.Doctors.Dtos;
using MediatR;

namespace Booking.Application.Doctors.Queries.GetDoctors
{
    public class GetDoctorsQuery
                : IRequest<List<DoctorDto>>
    {
        public string? SearchTerm { get; init; }
        public string? Specialty { get; init; }
    }
}
