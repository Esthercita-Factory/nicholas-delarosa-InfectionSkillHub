namespace InfectionSkillHub.Domain.Errors;

/// <summary>The data is valid, but the operation breaks a business rule (capacity, deadlines, dates).</summary>
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message)
    {
    }
}
