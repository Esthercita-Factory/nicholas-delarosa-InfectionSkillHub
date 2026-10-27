using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Enrollments;

namespace InfectionSkillHub.Application.Services.Enrollments;

public interface IEnrollmentService
{
    /// <summary>Enrolls the authenticated participant in a workshop.</summary>
    Task<Result<EnrollmentResponse>> EnrollAsync(string userId, EnrollRequest request, CancellationToken cancellationToken = default);

    /// <summary>Enrollments of the authenticated participant, active and cancelled.</summary>
    Task<Result<IReadOnlyList<EnrollmentResponse>>> ListMineAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result<EnrollmentResponse>> GetByIdAsync(Guid id, RequestingUser user, CancellationToken cancellationToken = default);

    Task<Result<EnrollmentResponse>> CancelAsync(Guid id, RequestingUser user, CancellationToken cancellationToken = default);
}
