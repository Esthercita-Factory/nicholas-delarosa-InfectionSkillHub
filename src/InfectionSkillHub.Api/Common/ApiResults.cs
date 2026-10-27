using System.Text.Json;
using InfectionSkillHub.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace InfectionSkillHub.Api.Common;

/// <summary>
/// Translates Application results into HTTP responses. Errors are returned as ProblemDetails
/// (RFC 9457); validation errors with a field also fill "errors", like [ApiController] does.
/// </summary>
public static class ApiResults
{
    public static ActionResult<T> OkOrProblem<T>(this ControllerBase controller, Result<T> result)
        => result.IsSuccess ? controller.Ok(result.Value) : controller.ProblemFor(result.Error!);

    public static ObjectResult ProblemFor(this ControllerBase controller, Error error)
    {
        var status = StatusFor(error.Code);

        var problem = error.Field is not null && error.Code is ErrorCode.Validation or ErrorCode.Conflict
            ? new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [JsonNamingPolicy.CamelCase.ConvertName(error.Field)] = [error.Message]
            })
            : new ProblemDetails();

        problem.Status = status;
        problem.Title = TitleFor(error.Code);
        problem.Detail = error.Message;
        problem.Instance = controller.HttpContext?.Request.Path.Value;

        return new ObjectResult(problem)
        {
            StatusCode = status,
            ContentTypes = { "application/problem+json" }
        };
    }

    public static int StatusFor(ErrorCode code) => code switch
    {
        ErrorCode.Validation => StatusCodes.Status400BadRequest,
        ErrorCode.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorCode.Forbidden => StatusCodes.Status403Forbidden,
        ErrorCode.NotFound => StatusCodes.Status404NotFound,
        ErrorCode.Conflict => StatusCodes.Status409Conflict,
        ErrorCode.BusinessRule => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string TitleFor(ErrorCode code) => code switch
    {
        ErrorCode.Validation => "Invalid data",
        ErrorCode.Unauthorized => "Not authenticated",
        ErrorCode.Forbidden => "Access denied",
        ErrorCode.NotFound => "Resource not found",
        ErrorCode.Conflict => "Conflict with existing data",
        ErrorCode.BusinessRule => "Business rule violated",
        _ => "Unexpected error"
    };
}
