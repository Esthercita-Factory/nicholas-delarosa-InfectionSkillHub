using InfectionSkillHub.Application.Abstractions;

namespace InfectionSkillHub.Infrastructure;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
