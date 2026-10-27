using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Errors;

namespace InfectionSkillHub.Domain.Services;

/// <summary>
/// Enrollment rules that need more than one entity: capacity, duplicates, schedule
/// overlaps and future workshops. The caller provides the current state of the data.
/// </summary>
public static class EnrollmentPolicy
{
    /// <param name="activeEnrollmentsInWorkshop">Active enrollments the workshop already has.</param>
    /// <param name="participantActiveWorkshops">Workshops where the participant already has an active enrollment.</param>
    public static Enrollment Enroll(
        Participant participant,
        Workshop workshop,
        int activeEnrollmentsInWorkshop,
        IReadOnlyCollection<Workshop> participantActiveWorkshops,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(participant);
        ArgumentNullException.ThrowIfNull(workshop);
        ArgumentNullException.ThrowIfNull(participantActiveWorkshops);

        if (workshop.HasStarted(now))
        {
            throw new BusinessRuleViolationException($"Workshop {workshop.Code} has already started; enrollments are closed.");
        }

        if (participantActiveWorkshops.Any(w => w.Id == workshop.Id))
        {
            throw new DuplicateEntityException($"The participant already has an active enrollment in workshop {workshop.Code}.");
        }

        if (activeEnrollmentsInWorkshop >= workshop.Capacity)
        {
            throw new BusinessRuleViolationException($"Workshop {workshop.Code} is full: all {workshop.Capacity} seats are taken.");
        }

        var overlapping = participantActiveWorkshops.FirstOrDefault(workshop.OverlapsWith);
        if (overlapping is not null)
        {
            throw new DuplicateEntityException(
                $"Workshop {workshop.Code} overlaps with workshop {overlapping.Code}, where the participant already has an active enrollment.");
        }

        return Enrollment.Create(workshop, participant, now);
    }

    public static int AvailableSeats(Workshop workshop, int activeEnrollments) => Math.Max(workshop.Capacity - activeEnrollments, 0);
}
