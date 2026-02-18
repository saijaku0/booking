using Booking.Application.Common.Interfaces;
using Booking.Application.Common.Security;
using Booking.Application.Doctors.Command.DeleteDoctor;
using MediatR;
using System.Reflection;

namespace Booking.Application.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse>(
        ICurrentUserService userService,
        IIdentityService identityService)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ICurrentUserService _userService = userService;
        private readonly IIdentityService _identityService = identityService;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var authorizeAttributes = request
                .GetType().GetCustomAttributes<AuthorizeAttribute>()
                .ToList();

            if (!authorizeAttributes.Any())
                return await next();  

            var userId = _userService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();

            var attributesWithRoles = authorizeAttributes
                .Where((a => a.Roles != null && a.Roles.Length > 0))
                .ToList();

            if (!attributesWithRoles.Any())
                return await next();

            
            if (await IsUserInAnyRoleAsync(userId, attributesWithRoles))
                return await next();

            throw new ForbiddenAccessException();
        }

        private async Task<bool> IsUserInAnyRoleAsync(
            string userId, 
            IEnumerable<AuthorizeAttribute> attributesWithRoles)
        {
            foreach (var attr in attributesWithRoles)
                foreach (var role in attr.Roles!)
                    if (await _identityService.IsInRoleAsync(userId, role.Trim()))
                        return true;

            return false;
        }
    }
}
