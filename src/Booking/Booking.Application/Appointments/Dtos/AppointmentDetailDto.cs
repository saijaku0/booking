
namespace Booking.Application.Appointments.Dtos
{
    public record AppointmentDetailDto
    {
        public Guid Id { get; init; }
        public Guid DoctorId { get; init; }
        public Guid PatientId { get; init; }

        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; init; } = string.Empty;
        public string Specialty { get; init; } = string.Empty;

        public string? DoctorPhotoUrl { get; init; }
        public string? DoctorPhoneNumber { get; init; }
        public decimal Price { get; init; }

        public string Status { get; init; } = string.Empty;
        public string? MedicalNotes {  get; init; } = string.Empty;
        public List<AttachmentDto> Attachments { get; init; } = [];
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
    }
}
