using InfectionSkillHub.Domain.Errors;

namespace InfectionSkillHub.Application.Common;

/// <summary>Expected failure of a use case. Field names the request property it belongs to, when there is one.</summary>
public sealed record Error(ErrorCode Code, string Message, string? Field = null)
{
    public static Error Validation(string message, string? field = null) => new(ErrorCode.Validation, message, field);

    public static Error NotFound(string message) => new(ErrorCode.NotFound, message);

    public static Error Conflict(string message, string? field = null) => new(ErrorCode.Conflict, message, field);

    public static Error BusinessRule(string message) => new(ErrorCode.BusinessRule, message);

    public static Error Unauthorized(string message) => new(ErrorCode.Unauthorized, message);

    public static Error Forbidden(string message) => new(ErrorCode.Forbidden, message);

    /// <summary>Translates a rule broken inside the domain into the error the API returns.</summary>
    public static Error FromDomain(DomainException exception) => exception switch
    {
        DuplicateEntityException => Conflict(exception.Message),
        BusinessRuleViolationException => BusinessRule(exception.Message),
        _ => Validation(exception.Message)
    };
}
