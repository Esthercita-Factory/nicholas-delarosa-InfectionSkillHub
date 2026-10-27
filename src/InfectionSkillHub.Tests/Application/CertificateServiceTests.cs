using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Certificates;
using InfectionSkillHub.Application.Services.Certificates;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Tests.Common;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace InfectionSkillHub.Tests.Application;

public class CertificateServiceTests
{
    private readonly IEnrollmentRepository _enrollments = Substitute.For<IEnrollmentRepository>();
    private readonly ICertificateGenerator _generator = Substitute.For<ICertificateGenerator>();
    private readonly CertificateService _service;

    public CertificateServiceTests()
    {
        _generator.Generate(Arg.Any<EnrollmentCertificate>()).Returns([1, 2, 3]);
        _service = new CertificateService(_enrollments, _generator, new FakeClock(), Options.Create(new InfectionSkillHubOptions()));
    }

    [Fact]
    public async Task GetAsync_PrintsTheWorkshopInTheLocalTimeOfTheCenter()
    {
        var owner = TestData.Participant();
        var enrollment = TestData.Enrollment(owner);
        _enrollments.FindByIdAsync(enrollment.Id, Arg.Any<CancellationToken>()).Returns(enrollment);

        var result = await _service.GetAsync(enrollment.Id, TestData.ParticipantUser(owner));

        Assert.True(result.IsSuccess);
        Assert.EndsWith(".pdf", result.Value!.FileName);
        _generator.Received(1).Generate(Arg.Is<EnrollmentCertificate>(c =>
            c.OrganizationName == "InfectionSkillHub"
            && c.ParticipantEmail == owner.Email
            && c.WorkshopCode == enrollment.Workshop.Code
            && c.StartsAt.Offset == TimeSpan.FromHours(-5)
            && c.StartsAt.UtcDateTime == enrollment.Workshop.StartsAt.UtcDateTime));
    }

    [Fact]
    public async Task GetAsync_HasNoCertificateForCancelledEnrollments()
    {
        var owner = TestData.Participant();
        var enrollment = TestData.Enrollment(owner);
        enrollment.Cancel(new CancellationActor(owner.UserId, owner.Email, false), TestData.Now);
        _enrollments.FindByIdAsync(enrollment.Id, Arg.Any<CancellationToken>()).Returns(enrollment);

        var result = await _service.GetAsync(enrollment.Id, TestData.ParticipantUser(owner));

        Assert.Equal(ErrorCode.BusinessRule, result.Error!.Code);
        _generator.DidNotReceiveWithAnyArgs().Generate(default!);
    }

    [Fact]
    public async Task GetAsync_ForbidsOtherParticipants()
    {
        var enrollment = TestData.Enrollment(TestData.Participant("owner@correo.com", "owner-id"));
        _enrollments.FindByIdAsync(enrollment.Id, Arg.Any<CancellationToken>()).Returns(enrollment);

        var result = await _service.GetAsync(enrollment.Id, TestData.ParticipantUser(TestData.Participant("other@correo.com", "other-id")));

        Assert.Equal(ErrorCode.Forbidden, result.Error!.Code);
    }

    [Fact]
    public async Task GetAsync_LetsEmployeesDownloadAnyCertificate()
    {
        var enrollment = TestData.Enrollment();
        _enrollments.FindByIdAsync(enrollment.Id, Arg.Any<CancellationToken>()).Returns(enrollment);

        var result = await _service.GetAsync(enrollment.Id, TestData.EmployeeUser());

        Assert.True(result.IsSuccess);
    }
}
