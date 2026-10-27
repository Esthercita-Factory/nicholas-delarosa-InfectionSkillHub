using InfectionSkillHub.Domain.Entities;

namespace InfectionSkillHub.Application.Common;

/// <summary>Authenticated user behind a request, taken from the token and never from the request body.</summary>
public sealed record RequestingUser(string UserId, string Email, bool IsEmployee)
{
    public CancellationActor ToCancellationActor() => new(UserId, Email, IsEmployee);
}
