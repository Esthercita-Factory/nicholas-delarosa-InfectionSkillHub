using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Errors;
using InfectionSkillHub.Domain.Services;

namespace InfectionSkillHub.Application.Services.Enrollments;

public sealed class EnrollmentRegistrar : IEnrollmentRegistrar
{
    private readonly IWorkshopRepository _workshops;
    private readonly IParticipantRepository _participants;
    private readonly IEnrollmentRepository _enrollments;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public EnrollmentRegistrar(
        IWorkshopRepository workshops,
        IParticipantRepository participants,
        IEnrollmentRepository enrollments,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _workshops = workshops;
        _participants = participants;
        _enrollments = enrollments;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result<Enrollment>> EnrollAsync(Participant participant, Workshop workshop, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Always workshop first, then participant: the same order in every request avoids deadlocks.
            await _workshops.LockAsync(workshop.Id, cancellationToken);
            await _participants.LockAsync(participant.Id, cancellationToken);

            var activeInWorkshop = await _enrollments.CountActiveAsync(workshop.Id, cancellationToken);
            var participantWorkshops = await _enrollments.ListActiveWorkshopsOfParticipantAsync(participant.Id, cancellationToken);

            var enrollment = EnrollmentPolicy.Enroll(participant, workshop, activeInWorkshop, participantWorkshops, _clock.UtcNow);

            await _enrollments.AddAsync(enrollment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return enrollment;
        }
        catch (DomainException exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            _unitOfWork.DiscardChanges();
            return Error.FromDomain(exception);
        }
    }
}
