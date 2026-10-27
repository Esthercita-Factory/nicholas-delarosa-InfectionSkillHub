using System.Net.Mail;
using InfectionSkillHub.Domain.Errors;

namespace InfectionSkillHub.Domain.Entities;

internal static class Guard
{
    public static string Required(string? value, string field, int maxLength)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        if (trimmed.Length == 0)
        {
            throw new DomainException($"{field} is required.");
        }

        if (trimmed.Length > maxLength)
        {
            throw new DomainException($"{field} cannot exceed {maxLength} characters.");
        }

        return trimmed;
    }

    public static string Email(string? value, int maxLength)
    {
        var email = Required(value, "Email", maxLength).ToLowerInvariant();

        if (!MailAddress.TryCreate(email, out var parsed) || parsed.Address != email)
        {
            throw new DomainException($"'{email}' is not a valid email address.");
        }

        return email;
    }
}
