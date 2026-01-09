using Booking.Application.Doctor.Dtos;
using MediatR;

namespace Booking.Application.Doctor.Queries.GetDoctors
{
    public class GetDoctorsQuery(string? searchTerm = null, string? specialty = null)
                : IRequest<List<DoctorDto>>
    {
        public string? SearchTerm { get; set; } = searchTerm;
        public string? Specialty { get; set; } = specialty;
    }
}
