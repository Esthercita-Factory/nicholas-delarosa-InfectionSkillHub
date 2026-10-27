using InfectionSkillHub.Domain.Entities;

namespace InfectionSkillHub.Application.Abstractions;

public interface IInstructorRepository
{
    Task<Instructor?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Instructor?> FindByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Instructor>> ListAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Instructor instructor, CancellationToken cancellationToken = default);
}
