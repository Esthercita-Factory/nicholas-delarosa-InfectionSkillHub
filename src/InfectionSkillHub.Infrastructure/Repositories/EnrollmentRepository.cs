using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Enums;
using InfectionSkillHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfectionSkillHub.Infrastructure.Repositories;

public sealed class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _db;

    public EnrollmentRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Enrollment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Enrollments
            .Include(e => e.Participant)
            .Include(e => e.Workshop)
            .ThenInclude(w => w.Instructor)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<int> CountActiveAsync(Guid workshopId, CancellationToken cancellationToken = default)
        => _db.Enrollments.CountAsync(e => e.WorkshopId == workshopId && e.Status == EnrollmentStatus.Active, cancellationToken);

    public async Task<IReadOnlyList<Workshop>> ListActiveWorkshopsOfParticipantAsync(Guid participantId, CancellationToken cancellationToken = default)
        => await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.ParticipantId == participantId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.Workshop)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Enrollment>> ListByParticipantAsync(Guid participantId, CancellationToken cancellationToken = default)
        => await _db.Enrollments
            .AsNoTracking()
            .Include(e => e.Participant)
            .Include(e => e.Workshop)
            .ThenInclude(w => w.Instructor)
            .Where(e => e.ParticipantId == participantId)
            .OrderByDescending(e => e.Workshop.StartsAt)
            .ThenByDescending(e => e.EnrolledAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Enrollment>> ListByWorkshopAsync(Guid workshopId, bool includeCancelled, CancellationToken cancellationToken = default)
        => await _db.Enrollments
            .AsNoTracking()
            .Include(e => e.Participant)
            .Where(e => e.WorkshopId == workshopId && (includeCancelled || e.Status == EnrollmentStatus.Active))
            .OrderBy(e => e.EnrolledAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
        => await _db.Enrollments.AddAsync(enrollment, cancellationToken);
}
