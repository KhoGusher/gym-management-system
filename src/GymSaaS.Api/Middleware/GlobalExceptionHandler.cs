using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GymSaaS.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, errors) = MapException(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            // Log full details server-side; never leak stack traces to the client.
            _logger.LogError(exception, "Unhandled exception occurred");
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.io/{statusCode}",
            Instance = httpContext.Request.Path
        };

        if (errors is not null)
            problemDetails.Extensions["errors"] = errors;
            problemDetails.Extensions["success"] = false;


        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // true = "handled", stop further processing
    }

    private static (int StatusCode, string Title, object? Errors) MapException(Exception exception) => exception switch
    {
        ValidationException validationEx => (
            StatusCodes.Status400BadRequest,
            "One or more validation errors occurred.",
            validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
        ),

        KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found.", null),

        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized.", null),

        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null)
    };
}