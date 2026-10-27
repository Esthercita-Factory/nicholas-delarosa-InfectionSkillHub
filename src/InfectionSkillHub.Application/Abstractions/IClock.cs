namespace InfectionSkillHub.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
