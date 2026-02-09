using Booking.Application.Appointments.Dtos;
using MediatR;

namespace Booking.Application.Appointments.Queries.GetAppointmentReport
{
    public record GetAppointmentReportQuery(Guid AppointmentId) 
        : IRequest<ExportFileDto>;
}
