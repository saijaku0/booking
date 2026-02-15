using Booking.Application.Common.Models;
using MediatR;

namespace Booking.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<string?> GetUserNameAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<bool> AuthorizeAsync(string userId, string policyName);
        Task<Result> DisableUserAsync(string userId);
    }
}
