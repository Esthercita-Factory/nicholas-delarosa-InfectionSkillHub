namespace InfectionSkillHub.Domain.Entities;

/// <summary>Person who enrolls in workshops. Linked to an Identity account with the Participant role.</summary>
public class Participant
{
    public const int FullNameMaxLength = 150;
    public const int EmailMaxLength = 256;

    private readonly List<Enrollment> _enrollments = [];

    private Participant()
    {
    }

    public Guid Id { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public DateTimeOffset RegisteredAt { get; private set; }

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;

    public static Participant Create(string userId, string fullName, string email, DateTimeOffset now) => new()
    {
        Id = Guid.NewGuid(),
        UserId = Guard.Required(userId, "User id", 450),
        FullName = Guard.Required(fullName, "Full name", FullNameMaxLength),
        Email = Guard.Email(email, EmailMaxLength),
        RegisteredAt = now.ToUniversalTime()
    };
}
