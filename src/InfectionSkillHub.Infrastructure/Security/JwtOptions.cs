using System.ComponentModel.DataAnnotations;

namespace InfectionSkillHub.Infrastructure.Security;

/// <summary>"Jwt" section. The signing key only lives in appsettings.Development.json or in environment variables.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = "InfectionSkillHub.Api";

    [Required]
    public string Audience { get; set; } = "InfectionSkillHub.Clients";

    /// <summary>HMAC-SHA256 needs at least 256 bits: 32 characters or more.</summary>
    [Required(ErrorMessage = "Jwt:SigningKey is missing (set it in appsettings.Development.json or as an environment variable).")]
    [MinLength(32, ErrorMessage = "Jwt:SigningKey must have at least 32 characters.")]
    public string SigningKey { get; set; } = string.Empty;

    [Range(5, 1440)]
    public int AccessTokenMinutes { get; set; } = 120;
}
