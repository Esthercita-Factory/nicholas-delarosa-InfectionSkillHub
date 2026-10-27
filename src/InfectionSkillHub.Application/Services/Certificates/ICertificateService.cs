using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Certificates;

namespace InfectionSkillHub.Application.Services.Certificates;

public interface ICertificateService
{
    /// <summary>
    /// PDF certificate of an active enrollment. A participant only gets their own;
    /// an employee can get any of them.
    /// </summary>
    Task<Result<CertificateFile>> GetAsync(Guid enrollmentId, RequestingUser user, CancellationToken cancellationToken = default);
}
