using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Enrollments;
using InfectionSkillHub.Application.Services.Enrollments;
using InfectionSkillHub.Domain.Enums;
using InfectionSkillHub.Tests.Common;
using NSubstitute;

namespace InfectionSkillHub.Tests.Application;

public class EnrollmentServiceTests
{
    private readonly IEnrollmentRegistrar _registrar = Substitute.For<IEnrollmentRegistrar>();
    private readonly IEnrollmentRepository _enrollments = Substitute.For<IEnrollmentRepository>();
    private readonly IParticipantRepository _participants = Substitute.For<IParticipantRepository>();
    private readonly IWorkshopRepository _workshops = Substitute.For<IWorkshopRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly EnrollmentService _service;

    public EnrollmentServiceTests()
    {
        _service = new EnrollmentService(_registrar, _enrollments, _participants, _workshops, _unitOfWork, new FakeClock());
    }

    [Fact]
    public async Task EnrollAsync_RejectsAccountsWithoutParticipantRecord()
    {
        var result = await _service.EnrollAsync("employee-id", new EnrollRequest { WorkshopId = Guid.NewGuid() });

        Assert.Equal(ErrorCode.Forbidden, result.Error!.Code);
    }

    [Fact]
    public async Task EnrollAsync_ReturnsNotFoundForAnUnknownWorkshop()
    {
        var participant = TestData.Participant();
        _participants.FindByUserIdAsync(participant.UserId, Arg.Any<CancellationToken>()).Returns(participant);

        var result = await _service.EnrollAsync(participant.UserId, new EnrollRequest { WorkshopId = Guid.NewGuid() });

        Assert.Equal(ErrorCode.NotFound, result.Error!.Code);
    }

    [Fact]
    public async Task EnrollAsync_EnrollsTheParticipantFromTheToken()
    {
        var participant = TestData.Participant();
        var workshop = TestData.Workshop();
        var enrollment = TestData.Enrollment(participant, workshop);
        _participants.FindByUserIdAsync(participant.UserId, Arg.Any<CancellationToken>()).Returns(participant);
        _workshops.FindByIdAsync(workshop.Id, Arg.Any<CancellationToken>()).Returns(workshop);
        _registrar.EnrollAsync(participant, workshop, Arg.Any<CancellationToken>()).Returns(Result.Success(enrollment));

        var result = await _service.EnrollAsync(participant.UserId, new EnrollRequest { WorkshopId = workshop.Id });

        Assert.True(result.IsSuccess);
        Assert.Equal(workshop.Code, result.Value!.Workshop.Code);
        Assert.True(result.Value.HasCertificate);
    }

    [Fact]
    public async Task CancelAsync_ForbidsCancellingSomebodyElsesEnrollment()
    {
        var enrollment = TestData.Enrollment(TestData.Participant("owner@correo.com", "owner-id"));
        _enrollments.FindByIdAsync(enrollment.Id, Arg.Any<CancellationToken>()).Returns(enrollment);
        var intruder = TestData.ParticipantUser(TestData.Participant("other@correo.com", "other-id"));

        var result = await _service.CancelAsync(enrollment.Id, intruder);

        Assert.Equal(ErrorCode.Forbidden, result.Error!.Code);
        Assert.True(enrollment.IsActive);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancelAsync_LetsTheOwnerCancel()
    {
        var owner = TestData.Participant();
        var enrollment = TestData.Enrollment(owner);
        _enrollments.FindByIdAsync(enrollment.Id, Arg.Any<CancellationToken>()).Returns(enrollment);

        var result = await _service.CancelAsync(enrollment.Id, TestData.ParticipantUser(owner));

        Assert.Equal(EnrollmentStatus.Cancelled, result.Value!.Status);
        Assert.Equal(owner.Email, result.Value.CancelledBy);
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancelAsync_LetsAnEmployeeCancelInsideTheLast24Hours()
    {
        var clock = new FakeClock();
        var service = new EnrollmentService(_registrar, _enrollments, _participants, _workshops, _unitOfWork, clock);
        var enrollment = TestData.Enrollment(workshop: TestData.Workshop(startsIn: TimeSpan.FromDays(2)));
        _enrollments.FindByIdAsync(enrollment.Id, Arg.Any<CancellationToken>()).Returns(enrollment);
        clock.UtcNow = enrollment.Workshop.StartsAt.AddHours(-2);
        var employee = TestData.EmployeeUser();

        var result = await service.CancelAsync(enrollment.Id, employee);

        Assert.True(result.IsSuccess);
        Assert.Equal(employee.Email, result.Value!.CancelledBy);
    }

    [Fact]
    public async Task CancelAsync_ReturnsBusinessRuleWhenTheParticipantIsTooLate()
    {
        var clock = new FakeClock();
        var service = new EnrollmentService(_registrar, _enrollments, _participants, _workshops, _unitOfWork, clock);
        var owner = TestData.Participant();
        var enrollment = TestData.Enrollment(owner, TestData.Workshop(startsIn: TimeSpan.FromDays(2)));
        _enrollments.FindByIdAsync(enrollment.Id, Arg.Any<CancellationToken>()).Returns(enrollment);
        clock.UtcNow = enrollment.Workshop.StartsAt.AddHours(-10);

        var result = await service.CancelAsync(enrollment.Id, TestData.ParticipantUser(owner));

        Assert.Equal(ErrorCode.BusinessRule, result.Error!.Code);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFoundForAnUnknownEnrollment()
    {
        var result = await _service.GetByIdAsync(Guid.NewGuid(), TestData.EmployeeUser());

        Assert.Equal(ErrorCode.NotFound, result.Error!.Code);
    }

    [Fact]
    public async Task ListMineAsync_ReturnsOnlyTheParticipantsOwnEnrollments()
    {
        var participant = TestData.Participant();
        _participants.FindByUserIdAsync(participant.UserId, Arg.Any<CancellationToken>()).Returns(participant);
        _enrollments.ListByParticipantAsync(participant.Id, Arg.Any<CancellationToken>()).Returns([TestData.Enrollment(participant)]);

        var result = await _service.ListMineAsync(participant.UserId);

        Assert.Single(result.Value!);
        await _enrollments.Received(1).ListByParticipantAsync(participant.Id, Arg.Any<CancellationToken>());
    }
}
