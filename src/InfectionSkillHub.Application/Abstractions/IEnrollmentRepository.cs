using InfectionSkillHub.Domain.Entities;

namespace InfectionSkillHub.Application.Abstractions;

public interface IEnrollmentRepository
{
    /// <summary>Includes the participant and the workshop with its instructor.</summary>
    Task<Enrollment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> CountActiveAsync(Guid workshopId, CancellationToken cancellationToken = default);

    /// <summary>Workshops where the participant has an active enrollment.</summary>
    Task<IReadOnlyList<Workshop>> ListActiveWorkshopsOfParticipantAsync(Guid participantId, CancellationToken cancellationToken = default);

    /// <summary>Every enrollment of the participant (active and cancelled), newest workshops first. Includes the workshop and its instructor.</summary>
    Task<IReadOnlyList<Enrollment>> ListByParticipantAsync(Guid participantId, CancellationToken cancellationToken = default);

    /// <summary>Enrollments of a workshop ordered by enrollment date. Includes the participant.</summary>
    Task<IReadOnlyList<Enrollment>> ListByWorkshopAsync(Guid workshopId, bool includeCancelled, CancellationToken cancellationToken = default);

    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
}
