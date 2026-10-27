using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Auth;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Errors;
using InfectionSkillHub.Domain.Identity;

namespace InfectionSkillHub.Application.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUserService _users;
    private readonly ITokenService _tokens;
    private readonly IParticipantRepository _participants;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public AuthService(
        IUserService users,
        ITokenService tokens,
        IParticipantRepository participants,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _users = users;
        _tokens = tokens;
        _participants = participants;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result<AuthResponse>> RegisterParticipantAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var fullName = request.FullName.Trim();

        if (await _users.EmailExistsAsync(email, cancellationToken))
        {
            return Error.Conflict("An account with this email already exists.", nameof(RegisterRequest.Email));
        }

        // The Identity account and the participant record are saved together or not at all.
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var creation = await _users.CreateAsync(
            new UserRegistration(email, fullName, request.Password),
            ApplicationRoles.Participant,
            cancellationToken);

        if (!creation.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Error.Validation(string.Join(" ", creation.Errors), nameof(RegisterRequest.Password));
        }

        try
        {
            var participant = Participant.Create(creation.User!.Id, fullName, email, _clock.UtcNow);
            await _participants.AddAsync(participant, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DomainException exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Error.FromDomain(exception);
        }

        return Session(creation.User!, [ApplicationRoles.Participant]);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var attempt = await _users.CheckPasswordAsync(request.Email.Trim().ToLowerInvariant(), request.Password, cancellationToken);

        switch (attempt.Outcome)
        {
            case SignInOutcome.LockedOut:
                return Error.Unauthorized("The account is temporarily locked after several failed attempts. Try again in a few minutes.");
            case SignInOutcome.InvalidCredentials:
                return Error.Unauthorized("Invalid email or password.");
        }

        var roles = await _users.GetRolesAsync(attempt.User!.Id, cancellationToken);
        return Session(attempt.User, roles);
    }

    public async Task<Result<UserProfile>> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _users.FindByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Error.NotFound("The account no longer exists.");
        }

        var roles = await _users.GetRolesAsync(user.Id, cancellationToken);
        return Profile(user, roles);
    }

    private AuthResponse Session(UserAccount user, IReadOnlyCollection<string> roles)
    {
        var token = _tokens.CreateToken(user, roles);
        return new AuthResponse(token.Token, token.ExpiresAt, Profile(user, roles));
    }

    private static UserProfile Profile(UserAccount user, IReadOnlyCollection<string> roles)
    {
        var role = roles.Contains(ApplicationRoles.Employee) ? ApplicationRoles.Employee : ApplicationRoles.Participant;
        return new UserProfile(user.Id, user.FullName, user.Email, role);
    }
}
