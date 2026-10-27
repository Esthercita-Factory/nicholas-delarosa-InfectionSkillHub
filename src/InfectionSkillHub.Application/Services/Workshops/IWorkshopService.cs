using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Workshops;

namespace InfectionSkillHub.Application.Services.Workshops;

public interface IWorkshopService
{
    /// <summary>Public catalog: only workshops that have not started, with their available seats.</summary>
    Task<IReadOnlyList<WorkshopResponse>> ListUpcomingAsync(CancellationToken cancellationToken = default);

    Task<Result<WorkshopResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<WorkshopResponse>> CreateAsync(CreateWorkshopRequest request, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<WorkshopEnrollmentResponse>>> ListEnrollmentsAsync(
        Guid workshopId,
        bool includeCancelled,
        CancellationToken cancellationToken = default);
}
