using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Domain.Entities;

namespace InfectionSkillHub.Application.Services.Enrollments;

/// <summary>
/// Single entry point to enroll a participant, shared by the participant endpoint and the
/// Excel import so both apply exactly the same rules.
/// </summary>
public interface IEnrollmentRegistrar
{
    Task<Result<Enrollment>> EnrollAsync(Participant participant, Workshop workshop, CancellationToken cancellationToken = default);
}
