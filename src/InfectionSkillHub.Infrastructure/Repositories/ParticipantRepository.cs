using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfectionSkillHub.Infrastructure.Repositories;

public sealed class ParticipantRepository : IParticipantRepository
{
    private readonly ApplicationDbContext _db;

    public ParticipantRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Participant?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return _db.Participants.FirstOrDefaultAsync(p => p.Email == normalized, cancellationToken);
    }

    public Task<Participant?> FindByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        => _db.Participants.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

    public Task LockAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Database.ExecuteSqlAsync($"SELECT 1 FROM participants WHERE id = {id} FOR UPDATE", cancellationToken);

    public async Task AddAsync(Participant participant, CancellationToken cancellationToken = default)
        => await _db.Participants.AddAsync(participant, cancellationToken);
}
