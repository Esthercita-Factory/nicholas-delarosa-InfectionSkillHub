using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Auth;

namespace InfectionSkillHub.Application.Services.Auth;

public interface IAuthService
{
    /// <summary>Public sign up. The account is always created as a participant and comes back signed in.</summary>
    Task<Result<AuthResponse>> RegisterParticipantAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<Result<UserProfile>> GetProfileAsync(string userId, CancellationToken cancellationToken = default);
}
