using MediatR;

namespace Booking.Application.Admin.Commands.CreateDoctor
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
    }
}
