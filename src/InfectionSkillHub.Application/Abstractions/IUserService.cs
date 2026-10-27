namespace InfectionSkillHub.Application.Abstractions;

/// <summary>Identity account as seen from Application, without depending on ASP.NET Core Identity.</summary>
public sealed record UserAccount(string Id, string Email, string FullName);

public sealed record UserRegistration(string Email, string FullName, string Password);

public sealed record UserCreation(UserAccount? User, IReadOnlyList<string> Errors)
{
    public bool Succeeded => User is not null;

    public static UserCreation Success(UserAccount user) => new(user, []);

    public static UserCreation Failure(IReadOnlyList<string> errors) => new(null, errors);
}

public enum SignInOutcome
{
    Success,
    InvalidCredentials,
    LockedOut
}

public sealed record SignInAttempt(SignInOutcome Outcome, UserAccount? User);

public interface IUserService
{
    Task<UserAccount?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Creates the account and assigns the role. Password policy errors come back in Errors.</summary>
    Task<UserCreation> CreateAsync(UserRegistration registration, string role, CancellationToken cancellationToken = default);

    /// <summary>Checks the password and applies lockout after repeated failures.</summary>
    Task<SignInAttempt> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolesAsync(string userId, CancellationToken cancellationToken = default);
}
