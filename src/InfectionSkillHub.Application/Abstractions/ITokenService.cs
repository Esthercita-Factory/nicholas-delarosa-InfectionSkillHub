namespace InfectionSkillHub.Application.Abstractions;

public sealed record AccessToken(string Token, DateTimeOffset ExpiresAt);

/// <summary>Issues the API access tokens (JWT, implemented in Infrastructure).</summary>
public interface ITokenService
{
    AccessToken CreateToken(UserAccount user, IReadOnlyCollection<string> roles);
}
