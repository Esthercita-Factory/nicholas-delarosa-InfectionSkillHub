using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Dtos.Instructors;
using InfectionSkillHub.Application.Mappings;

namespace InfectionSkillHub.Application.Services.Instructors;

public sealed class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructors;

    public InstructorService(IInstructorRepository instructors)
    {
        _instructors = instructors;
    }

    public async Task<IReadOnlyList<InstructorResponse>> ListAsync(CancellationToken cancellationToken = default)
        => (await _instructors.ListAsync(cancellationToken)).Select(i => i.ToResponse()).ToList();
}
