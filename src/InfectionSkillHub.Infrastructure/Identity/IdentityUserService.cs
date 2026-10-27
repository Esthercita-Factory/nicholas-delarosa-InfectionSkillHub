using InfectionSkillHub.Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace InfectionSkillHub.Infrastructure.Identity;

public sealed class IdentityUserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityUserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserAccount?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user is null ? null : ToAccount(user);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await _userManager.FindByEmailAsync(email) is not null;

    public async Task<UserCreation> CreateAsync(UserRegistration registration, string role, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = registration.Email,
            Email = registration.Email,
            EmailConfirmed = true,
            FullName = registration.FullName
        };

        var created = await _userManager.CreateAsync(user, registration.Password);
        if (!created.Succeeded)
        {
            return UserCreation.Failure(created.Errors.Select(e => e.Description).ToList());
        }

        var assigned = await _userManager.AddToRoleAsync(user, role);
        if (!assigned.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return UserCreation.Failure(assigned.Errors.Select(e => e.Description).ToList());
        }

        return UserCreation.Success(ToAccount(user));
    }

    public async Task<SignInAttempt> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return new SignInAttempt(SignInOutcome.InvalidCredentials, null);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return new SignInAttempt(SignInOutcome.LockedOut, null);
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.AccessFailedAsync(user);
            return await _userManager.IsLockedOutAsync(user)
                ? new SignInAttempt(SignInOutcome.LockedOut, null)
                : new SignInAttempt(SignInOutcome.InvalidCredentials, null);
        }

        await _userManager.ResetAccessFailedCountAsync(user);
        return new SignInAttempt(SignInOutcome.Success, ToAccount(user));
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? [] : (await _userManager.GetRolesAsync(user)).ToList();
    }

    private static UserAccount ToAccount(ApplicationUser user) => new(user.Id, user.Email ?? string.Empty, user.FullName);
}
