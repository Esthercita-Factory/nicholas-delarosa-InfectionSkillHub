using System.Text.RegularExpressions;
using InfectionSkillHub.Domain.Errors;

namespace InfectionSkillHub.Domain.Entities;

public partial class Workshop
{
    public const int CodeMaxLength = 20;
    public const int TitleMaxLength = 150;
    public const int DescriptionMaxLength = 1000;
    public const int MaxCapacity = 500;

    /// <summary>Participants can cancel their own enrollment up to this many hours before the start.</summary>
    public const int CancellationNoticeHours = 24;

    private readonly List<Enrollment> _enrollments = [];

    private Workshop()
    {
    }

    public Guid Id { get; private set; }

    /// <summary>Unique, always uppercase (for example FOT-001).</summary>
    public string Code { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public Guid InstructorId { get; private set; }

    public Instructor Instructor { get; private set; } = null!;

    /// <summary>Stored in UTC.</summary>
    public DateTimeOffset StartsAt { get; private set; }

    /// <summary>Stored in UTC.</summary>
    public DateTimeOffset EndsAt { get; private set; }

    public int Capacity { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;

    /// <summary>Last moment a participant can cancel their enrollment by themselves.</summary>
    public DateTimeOffset CancellationDeadline => StartsAt.AddHours(-CancellationNoticeHours);

    /// <summary>Publishes a new workshop. It must start in the future.</summary>
    public static Workshop Schedule(
        string code,
        string title,
        string description,
        Instructor instructor,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        int capacity,
        DateTimeOffset now)
    {
        if (startsAt <= now)
        {
            throw new BusinessRuleViolationException("A workshop must be scheduled with a future start date and time.");
        }

        return Build(code, title, description, instructor, startsAt, endsAt, capacity, now);
    }

    /// <summary>
    /// Loads a workshop that InfectionSkillHub had already published (initial data). It skips the future
    /// date rule, so the seed keeps working once those dates have passed; every other rule applies.
    /// </summary>
    public static Workshop Restore(
        string code,
        string title,
        string description,
        Instructor instructor,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        int capacity,
        DateTimeOffset now)
        => Build(code, title, description, instructor, startsAt, endsAt, capacity, now);

    public static string NormalizeCode(string? code) => (code ?? string.Empty).Trim().ToUpperInvariant();

    public bool HasStarted(DateTimeOffset now) => now >= StartsAt;

    /// <summary>Two workshops overlap when one starts before the other ends. Back-to-back sessions do not overlap.</summary>
    public bool OverlapsWith(Workshop other) => StartsAt < other.EndsAt && other.StartsAt < EndsAt;

    private static Workshop Build(
        string code,
        string title,
        string description,
        Instructor instructor,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        int capacity,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(instructor);

        var normalizedCode = Guard.Required(NormalizeCode(code), "Code", CodeMaxLength);
        if (!CodePattern().IsMatch(normalizedCode))
        {
            throw new DomainException("Code may only contain letters, digits and hyphens (for example FOT-001).");
        }

        if (endsAt <= startsAt)
        {
            throw new BusinessRuleViolationException("The end date and time must be later than the start date and time.");
        }

        if (capacity is < 1 or > MaxCapacity)
        {
            throw new DomainException($"Capacity must be between 1 and {MaxCapacity}.");
        }

        return new Workshop
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Title = Guard.Required(title, "Title", TitleMaxLength),
            Description = Guard.Required(description, "Description", DescriptionMaxLength),
            Instructor = instructor,
            InstructorId = instructor.Id,
            StartsAt = startsAt.ToUniversalTime(),
            EndsAt = endsAt.ToUniversalTime(),
            Capacity = capacity,
            CreatedAt = now.ToUniversalTime()
        };
    }

    [GeneratedRegex("^[A-Z0-9]+(-[A-Z0-9]+)*$")]
    private static partial Regex CodePattern();
}
