using Booking.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Booking.Infrastructure.Identity
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) 
        : ICurrentUserService
    {   
        private readonly IHttpContextAccessor _contextAccessor = httpContextAccessor;
        public string UserId => _contextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        public string UserName => _contextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        public bool IsAuthenticated => _contextAccessor.HttpContext?.User?
            .Identity?.IsAuthenticated ?? false;
    }
}
