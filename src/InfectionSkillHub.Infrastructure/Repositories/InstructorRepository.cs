using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfectionSkillHub.Infrastructure.Repositories;

public sealed class InstructorRepository : IInstructorRepository
{
    private readonly ApplicationDbContext _db;

    public InstructorRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Instructor?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return _db.Instructors.FirstOrDefaultAsync(i => i.Email == normalized, cancellationToken);
    }

    public Task<Instructor?> FindByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        => _db.Instructors.FirstOrDefaultAsync(i => i.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<Instructor>> ListAsync(CancellationToken cancellationToken = default)
        => await _db.Instructors
            .AsNoTracking()
            .OrderBy(i => i.FullName)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Instructor instructor, CancellationToken cancellationToken = default)
        => await _db.Instructors.AddAsync(instructor, cancellationToken);
}
