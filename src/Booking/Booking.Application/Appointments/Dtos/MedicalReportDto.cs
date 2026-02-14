namespace Booking.Application.Appointments.Dtos
{
    public record MedicalReportDto(
        string DoctorName,
        string DoctorSpecialty,
        string PatientName,
        DateTime Date,
        string Diagnosis,
        string? MedicalNotes,
        string? TreatmentPlan,
        string? PrescribedMedications);
}
