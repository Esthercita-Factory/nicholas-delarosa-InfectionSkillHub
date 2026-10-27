using InfectionSkillHub.Application.Dtos.Instructors;

namespace InfectionSkillHub.Application.Services.Instructors;

public interface IInstructorService
{
    Task<IReadOnlyList<InstructorResponse>> ListAsync(CancellationToken cancellationToken = default);
}
