using Booking.Application.Appointments.Dtos;

namespace Booking.Application.Appointments.Common.Interfaces
{
    public interface IPdfGenerator
    {
        byte[] GenerateMedicalReport(MedicalReportDto data);
    }
}
