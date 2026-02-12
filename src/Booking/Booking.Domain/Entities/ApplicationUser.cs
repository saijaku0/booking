using Microsoft.AspNetCore.Identity;

namespace Booking.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }

        public virtual Patient? PatientProfile { get; set; }
        public virtual Doctor? DoctorProfile { get; set; }
    }
}
