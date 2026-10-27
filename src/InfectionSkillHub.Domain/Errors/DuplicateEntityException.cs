namespace InfectionSkillHub.Domain.Errors;

/// <summary>The operation conflicts with data that already exists (duplicated code, enrollment or schedule).</summary>
public class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string message) : base(message)
    {
    }
}
