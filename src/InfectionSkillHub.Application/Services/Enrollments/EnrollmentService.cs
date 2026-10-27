using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Enrollments;
using InfectionSkillHub.Application.Mappings;
using InfectionSkillHub.Domain.Errors;

namespace InfectionSkillHub.Application.Services.Enrollments;

public sealed class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRegistrar _registrar;
    private readonly IEnrollmentRepository _enrollments;
    private readonly IParticipantRepository _participants;
    private readonly IWorkshopRepository _workshops;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public EnrollmentService(
        IEnrollmentRegistrar registrar,
        IEnrollmentRepository enrollments,
        IParticipantRepository participants,
        IWorkshopRepository workshops,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _registrar = registrar;
        _enrollments = enrollments;
        _participants = participants;
        _workshops = workshops;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result<EnrollmentResponse>> EnrollAsync(string userId, EnrollRequest request, CancellationToken cancellationToken = default)
    {
        var participant = await _participants.FindByUserIdAsync(userId, cancellationToken);
        if (participant is null)
        {
            return Error.Forbidden("Only participants can enroll in workshops.");
        }

        var workshopId = request.WorkshopId!.Value;
        var workshop = await _workshops.FindByIdAsync(workshopId, cancellationToken);
        if (workshop is null)
        {
            return Error.NotFound($"Workshop {workshopId} does not exist.");
        }

        var enrollment = await _registrar.EnrollAsync(participant, workshop, cancellationToken);
        return enrollment.IsSuccess ? enrollment.Value!.ToResponse(_clock.UtcNow) : enrollment.Error!;
    }

    public async Task<Result<IReadOnlyList<EnrollmentResponse>>> ListMineAsync(string userId, CancellationToken cancellationToken = default)
    {
        var participant = await _participants.FindByUserIdAsync(userId, cancellationToken);
        if (participant is null)
        {
            return Error.Forbidden("Only participants have their own enrollments.");
        }

        var now = _clock.UtcNow;
        var enrollments = await _enrollments.ListByParticipantAsync(participant.Id, cancellationToken);
        return Result.Success<IReadOnlyList<EnrollmentResponse>>(enrollments.Select(e => e.ToResponse(now)).ToList());
    }

    public async Task<Result<EnrollmentResponse>> GetByIdAsync(Guid id, RequestingUser user, CancellationToken cancellationToken = default)
    {
        var enrollment = await _enrollments.FindByIdAsync(id, cancellationToken);
        if (EnrollmentAccess.Check(enrollment, id, user) is { } denied)
        {
            return denied;
        }

        return enrollment!.ToResponse(_clock.UtcNow);
    }

    public async Task<Result<EnrollmentResponse>> CancelAsync(Guid id, RequestingUser user, CancellationToken cancellationToken = default)
    {
        var enrollment = await _enrollments.FindByIdAsync(id, cancellationToken);
        if (EnrollmentAccess.Check(enrollment, id, user) is { } denied)
        {
            return denied;
        }

        var now = _clock.UtcNow;

        try
        {
            enrollment!.Cancel(user.ToCancellationActor(), now);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DomainException exception)
        {
            return Error.FromDomain(exception);
        }

        return enrollment.ToResponse(now);
    }
}
