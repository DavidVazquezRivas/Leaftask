using BuildingBlocks.DrivingInfrastructure.Responses;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Host.Infrastructure;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        (int statusCode, string errorCode, string errorMessage) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "General.Validation",
                "Validation failed: " + string.Join(", ",
                    validationException.Errors.Select(e => $"Field: {e.PropertyName}, Error: {e.ErrorMessage}"))
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "General.InternalServerError",
                "An unexpected error occurred. Please try again later."
            )
        };

        ApiMeta meta = new()
        {
            Timestamp = DateTime.UtcNow,
            RequestId = httpContext.TraceIdentifier
        };

        ErrorDetails errorDetails = new(errorCode, errorMessage);

        ApiResponse response = ApiResponse.Failure(errorDetails, meta);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; // Mark exception as handled
    }
}
