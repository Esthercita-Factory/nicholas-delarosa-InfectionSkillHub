using InfectionSkillHub.Domain.Enums;
using InfectionSkillHub.Domain.Errors;
using InfectionSkillHub.Domain.Services;
using InfectionSkillHub.Tests.Common;

namespace InfectionSkillHub.Tests.Domain;

public class EnrollmentPolicyTests
{
    [Fact]
    public void Enroll_CreatesAnActiveEnrollment()
    {
        var participant = TestData.Participant();
        var workshop = TestData.Workshop();

        var enrollment = EnrollmentPolicy.Enroll(participant, workshop, 3, [], TestData.Now);

        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
        Assert.Equal(workshop.Id, enrollment.WorkshopId);
        Assert.Equal(participant.Id, enrollment.ParticipantId);
        Assert.Equal(TestData.Now, enrollment.EnrolledAt);
        Assert.Null(enrollment.CancelledAt);
    }

    [Fact]
    public void Enroll_RejectsAFullWorkshop()
    {
        var workshop = TestData.Workshop(capacity: 2);

        var exception = Assert.Throws<BusinessRuleViolationException>(
            () => EnrollmentPolicy.Enroll(TestData.Participant(), workshop, 2, [], TestData.Now));

        Assert.Contains("full", exception.Message);
    }

    [Fact]
    public void Enroll_AcceptsTheLastSeat()
    {
        var workshop = TestData.Workshop(capacity: 2);

        var enrollment = EnrollmentPolicy.Enroll(TestData.Participant(), workshop, 1, [], TestData.Now);

        Assert.True(enrollment.IsActive);
    }

    [Fact]
    public void Enroll_RejectsADuplicateActiveEnrollment()
    {
        var workshop = TestData.Workshop();

        Assert.Throws<DuplicateEntityException>(
            () => EnrollmentPolicy.Enroll(TestData.Participant(), workshop, 1, [workshop], TestData.Now));
    }

    [Fact]
    public void Enroll_RejectsAScheduleOverlap()
    {
        var current = TestData.Workshop("FOT-001", TimeSpan.FromDays(5), TimeSpan.FromHours(4));
        var overlapping = TestData.Workshop("COC-001", TimeSpan.FromDays(5) + TimeSpan.FromHours(2), TimeSpan.FromHours(3));

        var exception = Assert.Throws<DuplicateEntityException>(
            () => EnrollmentPolicy.Enroll(TestData.Participant(), overlapping, 0, [current], TestData.Now));

        Assert.Contains("FOT-001", exception.Message);
    }

    [Fact]
    public void Enroll_AllowsWorkshopsOnDifferentSchedules()
    {
        var current = TestData.Workshop("FOT-001", TimeSpan.FromDays(5), TimeSpan.FromHours(4));
        var later = TestData.Workshop("COC-001", TimeSpan.FromDays(5) + TimeSpan.FromHours(4), TimeSpan.FromHours(3));

        var enrollment = EnrollmentPolicy.Enroll(TestData.Participant(), later, 0, [current], TestData.Now);

        Assert.True(enrollment.IsActive);
    }

    [Fact]
    public void Enroll_RejectsAWorkshopThatAlreadyStarted()
    {
        var workshop = TestData.Workshop(startsIn: TimeSpan.FromHours(1));

        Assert.Throws<BusinessRuleViolationException>(
            () => EnrollmentPolicy.Enroll(TestData.Participant(), workshop, 0, [], workshop.StartsAt.AddMinutes(1)));
    }

    [Fact]
    public void AvailableSeats_NeverGoesBelowZero()
    {
        var workshop = TestData.Workshop(capacity: 5);

        Assert.Equal(2, EnrollmentPolicy.AvailableSeats(workshop, 3));
        Assert.Equal(0, EnrollmentPolicy.AvailableSeats(workshop, 7));
    }
}
