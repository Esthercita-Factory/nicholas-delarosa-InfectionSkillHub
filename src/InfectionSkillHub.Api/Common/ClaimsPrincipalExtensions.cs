using System.Security.Claims;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Domain.Identity;
using InfectionSkillHub.Infrastructure.Security;

namespace InfectionSkillHub.Api.Common;

public static class ClaimsPrincipalExtensions
{
    /// <summary>User id taken from the token. Participant routes never receive it in the request.</summary>
    public static string UserId(this ClaimsPrincipal user)
        => user.FindFirstValue(JwtTokenService.SubjectClaim)
            ?? throw new InvalidOperationException("The token has no sub claim.");

    public static RequestingUser ToRequestingUser(this ClaimsPrincipal user)
        => new(
            user.UserId(),
            user.FindFirstValue(JwtTokenService.EmailClaim) ?? string.Empty,
            user.IsInRole(ApplicationRoles.Employee));
}
