using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Enums;
using InfectionSkillHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfectionSkillHub.Infrastructure.Repositories;

public sealed class WorkshopRepository : IWorkshopRepository
{
    private readonly ApplicationDbContext _db;

    public WorkshopRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<WorkshopSeats>> ListUpcomingAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        var workshops = await _db.Workshops
            .AsNoTracking()
            .Include(w => w.Instructor)
            .Where(w => w.StartsAt > now)
            .OrderBy(w => w.StartsAt)
            .ThenBy(w => w.Code)
            .ToListAsync(cancellationToken);

        return await WithSeatsAsync(workshops, cancellationToken);
    }

    public async Task<WorkshopSeats?> FindWithSeatsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workshop = await _db.Workshops
            .AsNoTracking()
            .Include(w => w.Instructor)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        return workshop is null ? null : (await WithSeatsAsync([workshop], cancellationToken))[0];
    }

    public Task<Workshop?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Workshops
            .Include(w => w.Instructor)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public Task<Workshop?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = Workshop.NormalizeCode(code);
        return _db.Workshops
            .Include(w => w.Instructor)
            .FirstOrDefaultAsync(w => w.Code == normalized, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = Workshop.NormalizeCode(code);
        return _db.Workshops.AnyAsync(w => w.Code == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkshopSeats>> ListTopByActiveEnrollmentsAsync(int limit, CancellationToken cancellationToken = default)
    {
        var ranking = await _db.Workshops
            .Select(w => new
            {
                w.Id,
                w.Code,
                w.StartsAt,
                Active = w.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)
            })
            .OrderByDescending(r => r.Active)
            .ThenBy(r => r.StartsAt)
            .ThenBy(r => r.Code)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var ids = ranking.Select(r => r.Id).ToList();
        var workshops = await _db.Workshops
            .AsNoTracking()
            .Include(w => w.Instructor)
            .Where(w => ids.Contains(w.Id))
            .ToDictionaryAsync(w => w.Id, cancellationToken);

        return ranking.Select(r => new WorkshopSeats(workshops[r.Id], r.Active)).ToList();
    }

    public Task LockAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Database.ExecuteSqlAsync($"SELECT 1 FROM workshops WHERE id = {id} FOR UPDATE", cancellationToken);

    public async Task AddAsync(Workshop workshop, CancellationToken cancellationToken = default)
        => await _db.Workshops.AddAsync(workshop, cancellationToken);

    private async Task<IReadOnlyList<WorkshopSeats>> WithSeatsAsync(IReadOnlyList<Workshop> workshops, CancellationToken cancellationToken)
    {
        var ids = workshops.Select(w => w.Id).ToList();
        var active = await _db.Enrollments
            .Where(e => ids.Contains(e.WorkshopId) && e.Status == EnrollmentStatus.Active)
            .GroupBy(e => e.WorkshopId)
            .Select(g => new { WorkshopId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.WorkshopId, g => g.Count, cancellationToken);

        return workshops.Select(w => new WorkshopSeats(w, active.GetValueOrDefault(w.Id))).ToList();
    }
}
