using InfectionSkillHub.Domain.Entities;

namespace InfectionSkillHub.Application.Abstractions;

public interface IParticipantRepository
{
    Task<Participant?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Participant?> FindByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Locks the participant row until the current transaction ends, so two simultaneous
    /// enrollments of the same person cannot skip the schedule overlap check.
    /// </summary>
    Task LockAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Participant participant, CancellationToken cancellationToken = default);
}
