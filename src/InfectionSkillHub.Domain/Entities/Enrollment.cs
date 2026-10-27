using InfectionSkillHub.Domain.Enums;
using InfectionSkillHub.Domain.Errors;

namespace InfectionSkillHub.Domain.Entities;

/// <summary>
/// Seat of a participant in a workshop. It is born Active and can only move to Cancelled.
/// A cancelled enrollment is kept as history; re-enrolling creates a new one.
/// </summary>
public class Enrollment
{
    private Enrollment()
    {
    }

    public Guid Id { get; private set; }

    public Guid WorkshopId { get; private set; }

    public Workshop Workshop { get; private set; } = null!;

    public Guid ParticipantId { get; private set; }

    public Participant Participant { get; private set; } = null!;

    public EnrollmentStatus Status { get; private set; }

    public DateTimeOffset EnrolledAt { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

    /// <summary>Identity account that cancelled the enrollment (participant or employee).</summary>
    public string? CancelledByUserId { get; private set; }

    /// <summary>Email of whoever cancelled, kept as an audit snapshot.</summary>
    public string? CancelledByEmail { get; private set; }

    public bool IsActive => Status == EnrollmentStatus.Active;

    /// <summary>Only <see cref="Services.EnrollmentPolicy"/> creates enrollments, after checking every rule.</summary>
    internal static Enrollment Create(Workshop workshop, Participant participant, DateTimeOffset now) => new()
    {
        Id = Guid.NewGuid(),
        Workshop = workshop,
        WorkshopId = workshop.Id,
        Participant = participant,
        ParticipantId = participant.Id,
        Status = EnrollmentStatus.Active,
        EnrolledAt = now.ToUniversalTime()
    };

    /// <summary>
    /// Participants can cancel up to 24 hours before the start; employees can cancel any
    /// enrollment while the workshop has not started. Cancelling frees the seat.
    /// </summary>
    public void Cancel(CancellationActor actor, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(actor);

        if (!IsActive)
        {
            throw new BusinessRuleViolationException("This enrollment is already cancelled.");
        }

        if (Workshop.HasStarted(now))
        {
            throw new BusinessRuleViolationException("The workshop has already started, so the enrollment can no longer be cancelled.");
        }

        if (!actor.IsEmployee && now > Workshop.CancellationDeadline)
        {
            throw new BusinessRuleViolationException(
                $"Enrollments can only be cancelled up to {Workshop.CancellationNoticeHours} hours before the workshop starts.");
        }

        Status = EnrollmentStatus.Cancelled;
        CancelledAt = now.ToUniversalTime();
        CancelledByUserId = actor.UserId;
        CancelledByEmail = actor.Email;
    }

    /// <summary>A participant can cancel by themselves only while the enrollment is active and the deadline has not passed.</summary>
    public bool CanBeCancelledByParticipant(DateTimeOffset now) => IsActive && now <= Workshop.CancellationDeadline;
}
