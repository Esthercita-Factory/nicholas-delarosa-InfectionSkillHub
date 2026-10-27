using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Services.Enrollments;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Tests.Common;
using NSubstitute;

namespace InfectionSkillHub.Tests.Application;

public class EnrollmentRegistrarTests
{
    private readonly IWorkshopRepository _workshops = Substitute.For<IWorkshopRepository>();
    private readonly IParticipantRepository _participants = Substitute.For<IParticipantRepository>();
    private readonly IEnrollmentRepository _enrollments = Substitute.For<IEnrollmentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ITransactionScope _transaction = Substitute.For<ITransactionScope>();
    private readonly EnrollmentRegistrar _registrar;

    public EnrollmentRegistrarTests()
    {
        _unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(_transaction);
        _enrollments.ListActiveWorkshopsOfParticipantAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns([]);
        _registrar = new EnrollmentRegistrar(_workshops, _participants, _enrollments, _unitOfWork, new FakeClock());
    }

    [Fact]
    public async Task EnrollAsync_LocksSavesAndCommits()
    {
        var participant = TestData.Participant();
        var workshop = TestData.Workshop();

        var result = await _registrar.EnrollAsync(participant, workshop);

        Assert.True(result.IsSuccess);
        Received.InOrder(() =>
        {
            _workshops.LockAsync(workshop.Id, Arg.Any<CancellationToken>());
            _participants.LockAsync(participant.Id, Arg.Any<CancellationToken>());
            _enrollments.AddAsync(Arg.Any<Enrollment>(), Arg.Any<CancellationToken>());
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>());
            _transaction.CommitAsync(Arg.Any<CancellationToken>());
        });
    }

    [Fact]
    public async Task EnrollAsync_CountsOnlyActiveEnrollmentsForTheCapacity()
    {
        var workshop = TestData.Workshop(capacity: 3);
        _enrollments.CountActiveAsync(workshop.Id, Arg.Any<CancellationToken>()).Returns(3);

        var result = await _registrar.EnrollAsync(TestData.Participant(), workshop);

        Assert.Equal(ErrorCode.BusinessRule, result.Error!.Code);
        await _enrollments.DidNotReceive().AddAsync(Arg.Any<Enrollment>(), Arg.Any<CancellationToken>());
        await _transaction.Received().RollbackAsync(Arg.Any<CancellationToken>());
        _unitOfWork.Received().DiscardChanges();
    }

    [Fact]
    public async Task EnrollAsync_ReturnsConflictForAnOverlap()
    {
        var participant = TestData.Participant();
        var current = TestData.Workshop("FOT-001", TimeSpan.FromDays(4), TimeSpan.FromHours(4));
        var overlapping = TestData.Workshop("COC-001", TimeSpan.FromDays(4) + TimeSpan.FromHours(1), TimeSpan.FromHours(4));
        _enrollments.ListActiveWorkshopsOfParticipantAsync(participant.Id, Arg.Any<CancellationToken>()).Returns([current]);

        var result = await _registrar.EnrollAsync(participant, overlapping);

        Assert.Equal(ErrorCode.Conflict, result.Error!.Code);
    }
}
