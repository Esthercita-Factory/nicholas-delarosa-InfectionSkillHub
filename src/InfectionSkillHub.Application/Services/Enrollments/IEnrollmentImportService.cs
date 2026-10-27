using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Enrollments;

namespace InfectionSkillHub.Application.Services.Enrollments;

public interface IEnrollmentImportService
{
    /// <summary>
    /// Processes every row as a regular enrollment. Valid rows are saved even when others fail;
    /// the response tells how many were enrolled and why each rejected row failed.
    /// </summary>
    Task<Result<EnrollmentImportResponse>> ImportAsync(Stream content, CancellationToken cancellationToken = default);
}
