using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Enums;
using InfectionSkillHub.Domain.Errors;
using InfectionSkillHub.Tests.Common;

namespace InfectionSkillHub.Tests.Domain;

public class EnrollmentCancellationTests
{
    private static readonly CancellationActor Participant = new("participant-id", "jorge.perez@correo.com", false);
    private static readonly CancellationActor Employee = new("employee-id", "rosa.barrios@correo.com", true);

    [Fact]
    public void Cancel_ByTheParticipantBeforeTheDeadline_RecordsWhoAndWhen()
    {
        var enrollment = TestData.Enrollment(workshop: TestData.Workshop(startsIn: TimeSpan.FromDays(3)));
        var when = TestData.Now.AddHours(1);

        enrollment.Cancel(Participant, when);

        Assert.Equal(EnrollmentStatus.Cancelled, enrollment.Status);
        Assert.Equal(when, enrollment.CancelledAt);
        Assert.Equal(Participant.UserId, enrollment.CancelledByUserId);
        Assert.Equal(Participant.Email, enrollment.CancelledByEmail);
    }

    [Fact]
    public void Cancel_ByTheParticipantExactlyAtTheDeadline_IsAllowed()
    {
        var enrollment = TestData.Enrollment(workshop: TestData.Workshop(startsIn: TimeSpan.FromDays(3)));

        enrollment.Cancel(Participant, enrollment.Workshop.CancellationDeadline);

        Assert.False(enrollment.IsActive);
    }

    [Fact]
    public void Cancel_ByTheParticipantInsideTheLast24Hours_IsRejected()
    {
        var enrollment = TestData.Enrollment(workshop: TestData.Workshop(startsIn: TimeSpan.FromDays(3)));
        var tooLate = enrollment.Workshop.StartsAt.AddHours(-23);

        var exception = Assert.Throws<BusinessRuleViolationException>(() => enrollment.Cancel(Participant, tooLate));

        Assert.Contains("24 hours", exception.Message);
        Assert.True(enrollment.IsActive);
    }

    [Fact]
    public void Cancel_ByAnEmployeeInsideTheLast24Hours_IsAllowed()
    {
        var enrollment = TestData.Enrollment(workshop: TestData.Workshop(startsIn: TimeSpan.FromDays(3)));

        enrollment.Cancel(Employee, enrollment.Workshop.StartsAt.AddMinutes(-5));

        Assert.Equal(EnrollmentStatus.Cancelled, enrollment.Status);
        Assert.Equal(Employee.Email, enrollment.CancelledByEmail);
    }

    [Fact]
    public void Cancel_ByAnEmployeeOnceTheWorkshopStarted_IsRejected()
    {
        var enrollment = TestData.Enrollment(workshop: TestData.Workshop(startsIn: TimeSpan.FromDays(3)));

        Assert.Throws<BusinessRuleViolationException>(() => enrollment.Cancel(Employee, enrollment.Workshop.StartsAt));
    }

    [Fact]
    public void Cancel_AnAlreadyCancelledEnrollment_IsRejected()
    {
        var enrollment = TestData.Enrollment();
        enrollment.Cancel(Participant, TestData.Now);

        Assert.Throws<BusinessRuleViolationException>(() => enrollment.Cancel(Employee, TestData.Now));
    }

    [Fact]
    public void CanBeCancelledByParticipant_FollowsTheDeadline()
    {
        var enrollment = TestData.Enrollment(workshop: TestData.Workshop(startsIn: TimeSpan.FromDays(3)));

        Assert.True(enrollment.CanBeCancelledByParticipant(TestData.Now));
        Assert.False(enrollment.CanBeCancelledByParticipant(enrollment.Workshop.StartsAt.AddHours(-1)));
    }
}
