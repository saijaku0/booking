using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Booking.Application.Common.Exceptions
{
    public static class IdentityResultExtensions
    {
        public static void EnsureSucceeded(this IdentityResult result, string message)
        {
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e =>
                    new ValidationFailure(message, e.Description));

                throw new ValidationException(errors);
            }
        }
    }
}
