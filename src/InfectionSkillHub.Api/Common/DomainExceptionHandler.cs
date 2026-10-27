using InfectionSkillHub.Domain.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InfectionSkillHub.Api.Common;

/// <summary>
/// Last safety net: a business rule that escapes as an exception is returned as ProblemDetails;
/// any other exception becomes a 500 without internal details.
/// </summary>
public sealed class DomainExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<DomainExceptionHandler> _logger;

    public DomainExceptionHandler(IProblemDetailsService problemDetails, ILogger<DomainExceptionHandler> logger)
    {
        _problemDetails = problemDetails;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            DuplicateEntityException => (StatusCodes.Status409Conflict, "Conflict with existing data", exception.Message),
            BusinessRuleViolationException => (StatusCodes.Status422UnprocessableEntity, "Business rule violated", exception.Message),
            DomainException => (StatusCodes.Status400BadRequest, "Invalid data", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error", "An error occurred while processing the request.")
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled error on {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Business rule violated on {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = status;

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = status, Title = title, Detail = detail }
        });
    }
}
