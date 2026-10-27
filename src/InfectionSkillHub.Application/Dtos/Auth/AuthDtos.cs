using System.ComponentModel.DataAnnotations;
using InfectionSkillHub.Domain.Entities;

namespace InfectionSkillHub.Application.Dtos.Auth;

public sealed class RegisterRequest
{
    /// <example>Ana María Gómez Ruiz</example>
    [Required]
    [StringLength(Participant.FullNameMaxLength)]
    public string FullName { get; init; } = string.Empty;

    /// <example>ana.gomez@correo.com</example>
    [Required]
    [EmailAddress]
    [StringLength(Participant.EmailMaxLength)]
    public string Email { get; init; } = string.Empty;

    /// <summary>At least 8 characters with uppercase, lowercase, a digit and a symbol.</summary>
    /// <example>Skillhub2026*</example>
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}

public sealed class LoginRequest
{
    /// <example>andres.mejia@correo.com</example>
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    /// <example>Instructor2026*</example>
    [Required]
    public string Password { get; init; } = string.Empty;
}

/// <param name="Role">Employee or Participant.</param>
public sealed record UserProfile(string Id, string FullName, string Email, string Role);

/// <param name="AccessToken">JWT to send as "Authorization: Bearer {token}".</param>
public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, UserProfile User);
