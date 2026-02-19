using Booking.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Booking.API.Infrastructure;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException => CreateValidationProblemDetails(validationException),
            NotFoundException notFoundException => CreateNotFoundProblemDetails(notFoundException),
            ForbiddenAccessException forbiddenException => CreateForbiddenProblemDetails(forbiddenException),
            _ => CreateInternalServerErrorProblemDetails(exception)
        };

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        LogException(exception);

        return true;
    }

    private ProblemDetails CreateValidationProblemDetails(ValidationException ex)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = "One or more validation errors occurred."
        };
        problemDetails.Extensions["errors"] = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
        return problemDetails;
    }

    private ProblemDetails CreateNotFoundProblemDetails(NotFoundException ex)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Detail = ex.Message
        };
    }

    private ProblemDetails CreateForbiddenProblemDetails(ForbiddenAccessException ex)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Detail = ex.Message
        };
    }

    private ProblemDetails CreateInternalServerErrorProblemDetails(Exception ex)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server Error",
            Detail = "An unexpected error occurred."
        };
    }

    private void LogException(Exception ex)
    {
        switch (ex)
        {
            case ValidationException validationEx:
                logger.LogWarning(validationEx, "Validation error");
                break;
            case NotFoundException notFoundEx:
                logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
                break;
            case ForbiddenAccessException forbiddenEx:
                logger.LogWarning(forbiddenEx, "Access forbidden: {Message}", forbiddenEx.Message);
                break;
            default:
                logger.LogError(ex, "Unhandled exception");
                break;
        }
    }
}