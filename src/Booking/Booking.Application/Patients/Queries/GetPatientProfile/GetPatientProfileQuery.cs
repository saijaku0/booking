using Booking.Application.Patients.Dtos;
using MediatR;

namespace Booking.Application.Patients.Queries.GetPatientProfile
{
    public record GetPatientProfileQuery() : IRequest<PatientProfileDto>;

}
