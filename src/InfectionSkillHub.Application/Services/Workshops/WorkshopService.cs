using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Workshops;
using InfectionSkillHub.Application.Mappings;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Errors;

namespace InfectionSkillHub.Application.Services.Workshops;

public sealed class WorkshopService : IWorkshopService
{
    private readonly IWorkshopRepository _workshops;
    private readonly IInstructorRepository _instructors;
    private readonly IEnrollmentRepository _enrollments;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public WorkshopService(
        IWorkshopRepository workshops,
        IInstructorRepository instructors,
        IEnrollmentRepository enrollments,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _workshops = workshops;
        _instructors = instructors;
        _enrollments = enrollments;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<IReadOnlyList<WorkshopResponse>> ListUpcomingAsync(CancellationToken cancellationToken = default)
    {
        var now = _clock.UtcNow;
        var upcoming = await _workshops.ListUpcomingAsync(now, cancellationToken);
        return upcoming.Select(w => w.ToResponse(now)).ToList();
    }

    public async Task<Result<WorkshopResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var seats = await _workshops.FindWithSeatsAsync(id, cancellationToken);
        return seats is null ? WorkshopNotFound(id) : seats.ToResponse(_clock.UtcNow);
    }

    public async Task<Result<WorkshopResponse>> CreateAsync(CreateWorkshopRequest request, CancellationToken cancellationToken = default)
    {
        var instructorEmail = request.InstructorEmail.Trim().ToLowerInvariant();
        var instructor = await _instructors.FindByEmailAsync(instructorEmail, cancellationToken);
        if (instructor is null)
        {
            return Error.Validation(
                $"There is no instructor registered with the email {instructorEmail}.",
                nameof(CreateWorkshopRequest.InstructorEmail));
        }

        var code = Workshop.NormalizeCode(request.Code);
        if (await _workshops.CodeExistsAsync(code, cancellationToken))
        {
            return Error.Conflict($"A workshop with the code {code} already exists.", nameof(CreateWorkshopRequest.Code));
        }

        var now = _clock.UtcNow;

        try
        {
            var workshop = Workshop.Schedule(
                code,
                request.Title,
                request.Description,
                instructor,
                request.StartsAt!.Value,
                request.EndsAt!.Value,
                request.Capacity!.Value,
                now);

            await _workshops.AddAsync(workshop, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new WorkshopSeats(workshop, 0).ToResponse(now);
        }
        catch (DomainException exception)
        {
            return Error.FromDomain(exception);
        }
    }

    public async Task<Result<IReadOnlyList<WorkshopEnrollmentResponse>>> ListEnrollmentsAsync(
        Guid workshopId,
        bool includeCancelled,
        CancellationToken cancellationToken = default)
    {
        if (await _workshops.FindByIdAsync(workshopId, cancellationToken) is null)
        {
            return WorkshopNotFound(workshopId);
        }

        var enrollments = await _enrollments.ListByWorkshopAsync(workshopId, includeCancelled, cancellationToken);
        return Result.Success<IReadOnlyList<WorkshopEnrollmentResponse>>(
            enrollments.Select(e => e.ToWorkshopEnrollmentResponse()).ToList());
    }

    private static Error WorkshopNotFound(Guid id) => Error.NotFound($"Workshop {id} does not exist.");
}
