using MediatR;

namespace Booking.Application.Doctors.Command.CreateDoctor
{
    public record CreateDoctorCommand
        : IRequest<Guid>
    {
        public string Email { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Lastname {  get; set; } = string.Empty;
        public Guid SpecialtyId {  get; set; }
        public bool IsActive { get; set; }
        public decimal ConsultationFee { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
    }
}
