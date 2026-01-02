using FluentValidation;
using MediatR;

namespace Booking.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
        {
            if (!validators.Any()) 
                return await next();

            ValidationContext<TRequest> context = new(request);

            var validationResult = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellation))); 

            var failures = validationResult
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);

            return await next();
        }
    }
}
