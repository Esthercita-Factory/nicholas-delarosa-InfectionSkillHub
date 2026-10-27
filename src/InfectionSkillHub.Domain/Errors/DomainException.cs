namespace InfectionSkillHub.Domain.Errors;

/// <summary>Invalid data for an entity (empty fields, lengths, formats).</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
