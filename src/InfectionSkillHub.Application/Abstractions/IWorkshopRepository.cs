using InfectionSkillHub.Domain.Entities;

namespace InfectionSkillHub.Application.Abstractions;

/// <summary>Workshop together with the number of active enrollments it has.</summary>
public sealed record WorkshopSeats(Workshop Workshop, int ActiveEnrollments);

public interface IWorkshopRepository
{
    /// <summary>Workshops that have not started yet, ordered by start date. Includes the instructor.</summary>
    Task<IReadOnlyList<WorkshopSeats>> ListUpcomingAsync(DateTimeOffset now, CancellationToken cancellationToken = default);

    Task<WorkshopSeats?> FindWithSeatsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Includes the instructor.</summary>
    Task<Workshop?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Includes the instructor. The code is compared in uppercase.</summary>
    Task<Workshop?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Workshops ordered from most to fewest active enrollments.</summary>
    Task<IReadOnlyList<WorkshopSeats>> ListTopByActiveEnrollmentsAsync(int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Locks the workshop row until the current transaction ends, so two enrollments
    /// in the same workshop cannot both take the last seat.
    /// </summary>
    Task LockAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Workshop workshop, CancellationToken cancellationToken = default);
}
