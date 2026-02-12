using Booking.Application.Appointments.Dtos;
using Booking.Domain.Entities;

namespace Booking.Application.Appointments.Common.Interfaces
{
    public interface IPdfGenerator
    {
        byte[] GenerateMedicalReport(MedicalReportDto data);
        byte[] GeneratePrescription(MedicalReportDto data);
    }
}
