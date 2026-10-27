namespace InfectionSkillHub.Domain.Entities;

/// <summary>InfectionSkillHub employee who teaches workshops. Linked to an Identity account with the Employee role.</summary>
public class Instructor
{
    public const int FullNameMaxLength = 150;
    public const int EmailMaxLength = 256;
    public const int SpecialtyMaxLength = 100;

    private readonly List<Workshop> _workshops = [];

    private Instructor()
    {
    }

    public Guid Id { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Specialty { get; private set; } = string.Empty;

    public IReadOnlyCollection<Workshop> Workshops => _workshops;

    public static Instructor Create(string userId, string fullName, string email, string specialty) => new()
    {
        Id = Guid.NewGuid(),
        UserId = Guard.Required(userId, "User id", 450),
        FullName = Guard.Required(fullName, "Full name", FullNameMaxLength),
        Email = Guard.Email(email, EmailMaxLength),
        Specialty = Guard.Required(specialty, "Specialty", SpecialtyMaxLength)
    };
}
