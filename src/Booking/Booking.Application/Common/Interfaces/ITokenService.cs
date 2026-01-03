using Booking.Domain.Entities;

namespace Booking.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}
